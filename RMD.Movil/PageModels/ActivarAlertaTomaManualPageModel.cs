using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RMD.Movil.Core.Service.Interfaces;
using RMD.Movil.PageModels.Controls;
using RMD.Shared.Models.Consulta;
using RMD.Shared.Models.Receta.AlertaToma.Request;
using RMD.Shared.Models.Receta.Catalogos;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace RMD.Movil.PageModels
{
    public partial class ActivarAlertaTomaManualPageModel : BasePageModel
    {
        public sealed class DuracionOption
        {
            public string Text { get; init; } = "";
            public string Value { get; init; } = "";
        }

        private sealed class RelacionesResult
        {
            public List<string> Items { get; } = new();
            public Dictionary<string, int> Map { get; } = new(StringComparer.OrdinalIgnoreCase);
        }

        private readonly IAlertaTomaControllerService _alertasService;
        private readonly IRecetaCatalogosControllerService _catalogosService;
        private readonly IConsultaControllerService _consultaService;

        private Dictionary<string, int> _unidadesMap = new(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, int> _routesMap = new(StringComparer.OrdinalIgnoreCase);

        public ActivarAlertaTomaManualPageModel(
            IAlertaTomaControllerService alertasService,
            IRecetaCatalogosControllerService catalogosService,
            IConsultaControllerService consultaService)
        {
            _alertasService = alertasService;
            _catalogosService = catalogosService;
            _consultaService = consultaService;

            Unidades = new ObservableCollection<string>();
            PeriodosFrecuencia = new ObservableCollection<FrecuencyTypeListModel>();

            PeriodosDuracion = new ObservableCollection<DuracionOption>
            {
                new() { Text = "AÑO(S)", Value = "YEAR" },
                new() { Text = "MES(ES)", Value = "MONTH" },
                new() { Text = "SEMANA(S)", Value = "WEEK" },
                new() { Text = "DÍA(S)", Value = "DAY" },
                new() { Text = "HORA(S)", Value = "HOUR" },
                new() { Text = "MINUTO(S)", Value = "MINUTE" }
            };

            Vias = new ObservableCollection<string>();
            MotivosUso = new ObservableCollection<string>();

            PeriodoDuracionSeleccionado = PeriodosDuracion.First(o => o.Value == "DAY");

            HayMedicamentoSeleccionado = false;
            MostrarResultados = false;

            ActivarAlerta = true; // por defecto marcado
        }

        public async Task InitAsync() => await CargarFrecuencyTypesAsync();

        private async Task CargarFrecuencyTypesAsync()
        {
            try
            {
                if (IsBusy) return;
                IsBusy = true;

                var resp = await _catalogosService.GetFrecuencyTypesAsync();
                if (!EsOkToast(resp.Toast))
                {
                    await MostrarAlertPorRespuestaAsync(resp);
                    PeriodosFrecuencia = new();
                    PeriodoFrecuenciaSeleccionado = null;
                    return;
                }

                var lista = (resp.Data ?? new()).OrderBy(x => x.IdFrecuencyType).ToList();
                PeriodosFrecuencia = new ObservableCollection<FrecuencyTypeListModel>(lista);
                PeriodoFrecuenciaSeleccionado = PeriodosFrecuencia.FirstOrDefault();
            }
            catch (Exception ex)
            {
                await MostrarAlertAsync("Catálogo", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private const int MinSearchLen = 2;
        private const int DebounceMs = 450;
        private CancellationTokenSource? _searchCts;

        [ObservableProperty] private string? textoBusqueda;
        [ObservableProperty] private bool isSearching;
        [ObservableProperty] private bool mostrarResultados;
        [ObservableProperty] private ObservableCollection<Medicamentos> resultados = new();

        [ObservableProperty] private Medicamentos? medicamentoSeleccionado;

        partial void OnMedicamentoSeleccionadoChanged(Medicamentos? value)
        {
            MedicamentoSeleccionadoNombre = value?.Nombre;
            HayMedicamentoSeleccionado = false;
            _ = CargarRelacionesParaMedicamentoAsync(value);
            RecalcularPuedeGuardar();
        }

        [ObservableProperty] private string? medicamentoSeleccionadoNombre;
        [ObservableProperty] private bool hayMedicamentoSeleccionado;

        partial void OnTextoBusquedaChanged(string? value)
        {
            if (!string.IsNullOrWhiteSpace(MedicamentoSeleccionadoNombre) &&
                !string.Equals(MedicamentoSeleccionadoNombre, value ?? string.Empty, StringComparison.Ordinal))
            {
                MedicamentoSeleccionado = null;
            }

            DebouncedBuscar(value);
        }

        private async void DebouncedBuscar(string? value)
        {
            _searchCts?.Cancel();

            var q = (value ?? string.Empty).Trim();
            if (q.Length < MinSearchLen)
            {
                IsSearching = false;
                MostrarResultados = false;
                Resultados.Clear();
                return;
            }

            var cts = _searchCts = new CancellationTokenSource();
            try
            {
                IsSearching = true;
                await Task.Delay(DebounceMs, cts.Token);
                await BuscarMedicamentosCoreAsync(q, cts.Token);
            }
            catch (TaskCanceledException)
            {
            }
            finally
            {
                if (!cts.IsCancellationRequested)
                    IsSearching = false;
            }
        }

        private async Task BuscarMedicamentosCoreAsync(string q, CancellationToken ct)
        {
            try
            {
                var req = new BuscarPackageRequest { NombrePackage = q };
                var resp = await _alertasService.BuscarPackagePorNombreAsync(req);

                if (!EsOkToast(resp.Toast))
                {
                    await MostrarAlertPorRespuestaAsync(resp);
                    Resultados.Clear();
                    MostrarResultados = false;
                    return;
                }

                var lista = (resp.Data ?? new()).OrderBy(m => m.Nombre).ToList();
                if (ct.IsCancellationRequested) return;

                Resultados = new ObservableCollection<Medicamentos>(lista);
                MostrarResultados = Resultados.Count > 0;
            }
            catch (Exception ex)
            {
                if (!ct.IsCancellationRequested)
                    await MostrarAlertAsync("Búsqueda", ex.Message);
            }
        }

        [RelayCommand]
        private void SeleccionarMedicamento(Medicamentos? item)
        {
            if (item is null) return;

            MedicamentoSeleccionado = item;
            TextoBusqueda = item.Nombre;
            MostrarResultados = false;
        }

        [ObservableProperty] private string? cantidad;
        [ObservableProperty] private ObservableCollection<string> unidades;
        [ObservableProperty] private string? unidadSeleccionada;

        [ObservableProperty] private string? cadaValor;

        [ObservableProperty] private ObservableCollection<FrecuencyTypeListModel> periodosFrecuencia;
        [ObservableProperty] private FrecuencyTypeListModel? periodoFrecuenciaSeleccionado;

        [ObservableProperty] private bool esDosisUnica;

        [ObservableProperty] private string? duracionValor;

        [ObservableProperty] private ObservableCollection<DuracionOption> periodosDuracion;
        [ObservableProperty] private DuracionOption? periodoDuracionSeleccionado;

        [ObservableProperty] private ObservableCollection<string> vias;
        [ObservableProperty] private string? viaSeleccionada;

        [ObservableProperty] private ObservableCollection<string> motivosUso;
        [ObservableProperty] private string? motivoUsoSeleccionado;

        [ObservableProperty] private string? indicaciones;

        // NUEVO: checkbox “Activar alerta…”
        [ObservableProperty] private bool activarAlerta = true;

        // ======== VISIBILIDAD SOLICITADA ========
        [ObservableProperty] private bool mostrarVia;                   // depende de ROUTES
        [ObservableProperty] private bool mostrarMotivoUso;             // depende de INDICATIONS
        [ObservableProperty] private bool mostrarFilaCantidadUnidad;    // depende de UNITS
        // ========================================

        [ObservableProperty] private bool puedeGuardar;
        partial void OnCantidadChanged(string? value) => RecalcularPuedeGuardar();
        partial void OnCadaValorChanged(string? value) => RecalcularPuedeGuardar();
        partial void OnDuracionValorChanged(string? value) => RecalcularPuedeGuardar();
        partial void OnUnidadSeleccionadaChanged(string? value) => RecalcularPuedeGuardar();
        partial void OnPeriodoFrecuenciaSeleccionadoChanged(FrecuencyTypeListModel? value) => RecalcularPuedeGuardar();
        partial void OnPeriodoDuracionSeleccionadoChanged(DuracionOption? value) => RecalcularPuedeGuardar();
        partial void OnViaSeleccionadaChanged(string? value) => RecalcularPuedeGuardar();
        partial void OnMotivoUsoSeleccionadoChanged(string? value) => RecalcularPuedeGuardar();

        // Cuando cambian las colecciones, recalcula las banderas de visibilidad
        partial void OnUnidadesChanged(ObservableCollection<string> value) => RecalcularVisibilidades();
        partial void OnViasChanged(ObservableCollection<string> value) => RecalcularVisibilidades();
        partial void OnMotivosUsoChanged(ObservableCollection<string> value) => RecalcularVisibilidades();

        private void RecalcularVisibilidades()
        {
            MostrarFilaCantidadUnidad = Unidades is { Count: > 0 };
            MostrarVia = Vias is { Count: > 0 };
            MostrarMotivoUso = MotivosUso is { Count: > 0 };
        }

        private void RecalcularPuedeGuardar()
        {
            var unidadOk = !string.IsNullOrWhiteSpace(UnidadSeleccionada) &&
                           _unidadesMap.TryGetValue(UnidadSeleccionada, out var unidadId) &&
                           unidadId > 0;

            var viaOk = !string.IsNullOrWhiteSpace(ViaSeleccionada) &&
                        _routesMap.TryGetValue(ViaSeleccionada, out var viaId) &&
                        viaId > 0;

            // Si no hay UNITS, entonces la fila completa no existe y no se exige cantidad/unidad
            var requiereUnidad = MostrarFilaCantidadUnidad;
            var requiereVia = MostrarVia;

            var camposUnidadOk = !requiereUnidad || (!string.IsNullOrWhiteSpace(Cantidad) && unidadOk);
            var campoViaOk = !requiereVia || viaOk;

            PuedeGuardar =
                HayMedicamentoSeleccionado &&
                !string.IsNullOrWhiteSpace(DuracionValor) &&
                !string.IsNullOrWhiteSpace(CadaValor) &&
                PeriodoFrecuenciaSeleccionado != null &&
                PeriodoDuracionSeleccionado != null &&
                camposUnidadOk &&
                campoViaOk;
        }

        [RelayCommand]
        private async Task GuardarAsync()
        {
            if (!PuedeGuardar)
            {
                await MostrarAlertAsync("Validación", "Completa los campos requeridos.");
                return;
            }

            try
            {
                if (IsBusy) return;
                IsBusy = true;

                int.TryParse(CadaValor, out var cada);
                int.TryParse(DuracionValor, out var duracion);

                var cantidadDecimal = TryParseDecimal(Cantidad) ?? 0m;

                var unidadId = 0;
                if (MostrarFilaCantidadUnidad &&
                    _unidadesMap.TryGetValue(UnidadSeleccionada ?? "", out var uId))
                    unidadId = uId;

                var idFrecuencyType = PeriodoFrecuenciaSeleccionado!.IdFrecuencyType;

                var idRutaAdmin = 0;
                if (MostrarVia &&
                    _routesMap.TryGetValue(ViaSeleccionada ?? "", out var raId))
                    idRutaAdmin = raId;

                var notas = string.Join(" | ",
                    new[]
                    {
                        string.IsNullOrWhiteSpace(MotivoUsoSeleccionado) ? null : $"Motivo: {MotivoUsoSeleccionado}",
                        string.IsNullOrWhiteSpace(Indicaciones) ? null : Indicaciones
                    }.Where(s => !string.IsNullOrWhiteSpace(s)));

                var req = new ActivarAlertaManualRequest
                {
                    MedicamentoId = MedicamentoSeleccionado!.Id,
                    MedicamentoType = MedicamentoSeleccionado!.IdType,
                    CantidadDiaria = cantidadDecimal,
                    UnidadDispensacionId = unidadId,
                    IdRutaAdministracion = idRutaAdmin,
                    Duracion = duracion,
                    UnidadDuracion = PeriodoDuracionSeleccionado!.Value,
                    FechaHoraPrimerToma = DateTime.Now,
                    Frecuency = cada,
                    IdFrecuencyType = idFrecuencyType,
                    Notas = notas,
                    Activo = ActivarAlerta
                };

                var resp = await _alertasService.ActivarAlertaManualAsync(req);
                if (!EsOkToast(resp.Toast))
                {
                    await MostrarAlertPorRespuestaAsync(resp);
                    return;
                }

                await MostrarAlertAsync("Éxito", "Alerta manual activada.");
                await CloseAsync();
            }
            catch (Exception ex)
            {
                await MostrarAlertAsync("Guardar", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private Task CloseAsync() => Shell.Current.GoToAsync("..");

        private static decimal? TryParseDecimal(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            return decimal.TryParse(s, out var d) ? d : null;
        }

        private async Task CargarRelacionesParaMedicamentoAsync(Medicamentos? med)
        {
            if (med is null) return;

            try
            {
                if (IsBusy) return;
                IsBusy = true;

                var unitsTask = GetRelacionesAsync(med, "UNITS");
                var routesTask = GetRelacionesAsync(med, "ROUTES");
                var indicationsTask = GetRelacionesAsync(med, "INDICATIONS");

                await Task.WhenAll(unitsTask, routesTask, indicationsTask);

                _unidadesMap = unitsTask.Result.Map;
                Unidades = new ObservableCollection<string>(unitsTask.Result.Items);

                _routesMap = routesTask.Result.Map;
                Vias = new ObservableCollection<string>(routesTask.Result.Items);

                MotivosUso = new ObservableCollection<string>(indicationsTask.Result.Items);

                // Selecciones por defecto si existen
                UnidadSeleccionada = Unidades.FirstOrDefault();
                ViaSeleccionada = Vias.FirstOrDefault();
                MotivoUsoSeleccionado = MotivosUso.FirstOrDefault();

                // Recalcular banderas de visibilidad y validación
                RecalcularVisibilidades();
                HayMedicamentoSeleccionado = true;
                RecalcularPuedeGuardar();
            }
            catch (Exception ex)
            {
                await MostrarAlertAsync("Relaciones", ex.Message);
                HayMedicamentoSeleccionado = false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task<RelacionesResult> GetRelacionesAsync(Medicamentos med, string relacionType)
        {
            var resp = await _consultaService.GetIdsFromLinkAsync(med.Id, med.IdType, relacionType);
            var result = new RelacionesResult();

            if (!EsOkToast(resp.Toast))
            {
                await MostrarAlertPorRespuestaAsync(resp);
                return result;
            }

            void AddItem(string? name, int? id = null)
            {
                var n = (name ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(n)) return;
                if (!result.Items.Contains(n, StringComparer.OrdinalIgnoreCase))
                    result.Items.Add(n);
                if (id.HasValue && id.Value > 0)
                    result.Map[n] = id.Value;
            }

            if (resp.Data is JsonElement je)
            {
                if (je.ValueKind == JsonValueKind.Array)
                {
                    foreach (var el in je.EnumerateArray())
                    {
                        if (el.ValueKind == JsonValueKind.Object)
                        {
                            int? id = null;
                            string? name = null;

                            foreach (var prop in el.EnumerateObject())
                            {
                                var p = prop.Name.ToLowerInvariant();
                                if (p is "id" or "value" && prop.Value.ValueKind == JsonValueKind.Number && prop.Value.TryGetInt32(out var vid))
                                    id = vid;
                                else if (p is "name" or "nombre" or "text" && prop.Value.ValueKind == JsonValueKind.String)
                                    name = prop.Value.GetString();
                            }

                            if (name is not null) AddItem(name, id);
                        }
                        else if (el.ValueKind == JsonValueKind.String)
                        {
                            AddItem(el.GetString());
                        }
                        else
                        {
                            AddItem(el.ToString());
                        }
                    }
                }
                else if (je.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in je.EnumerateObject())
                    {
                        var key = prop.Name;
                        int? id = null;
                        if (prop.Value.ValueKind == JsonValueKind.Number && prop.Value.TryGetInt32(out var vid))
                            id = vid;
                        AddItem(key, id);
                    }
                }
                else if (je.ValueKind == JsonValueKind.String)
                {
                    AddItem(je.GetString());
                }
            }
            else if (resp.Data is IEnumerable<KeyValuePair<string, int>> kvps)
            {
                foreach (var kv in kvps) AddItem(kv.Key, kv.Value);
            }
            else if (resp.Data is IEnumerable<string> s1)
            {
                foreach (var s in s1) AddItem(s);
            }
            else if (resp.Data is IEnumerable s2)
            {
                foreach (var o in s2) AddItem(o?.ToString());
            }
            else if (resp.Data is not null)
            {
                AddItem(resp.Data.ToString());
            }

            return result;
        }
    }
}
