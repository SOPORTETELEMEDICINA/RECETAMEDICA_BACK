using RMD.Data;
using RMD.Interface.PuntoVenta;
using RMD.Models.PuntoVenta;

namespace RMD.Service
{
    public class PuntoVentaService(PuntoVentaDbContext context) : IPuntoVentaService
    {
        private readonly PuntoVentaDbContext _context = context;

        public async Task<(RecetaModel, List<DetalleRecetaModel>, PacienteModel, MedicoModel, GrupoEmpresarialModel)>
            ObtenerRecetaAsync(Guid idReceta, Guid idMedico, DateTime fechaUltimaModificacion)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("PuntoVenta_GetReceta", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Parámetros del SP
                command.Parameters.AddWithValue("@IdReceta", idReceta);
                command.Parameters.AddWithValue("@IdMedico", idMedico);
                command.Parameters.AddWithValue("@FechaUltimaModificacion", fechaUltimaModificacion);

                using var reader = await command.ExecuteReaderAsync();

                // Inicialización de modelos
                var receta = new RecetaModel();
                var detalles = new List<DetalleRecetaModel>();
                var paciente = new PacienteModel();
                var medico = new MedicoModel();
                var grupoEmpresarial = new GrupoEmpresarialModel();

                // Leer los datos de la receta
                if (await reader.ReadAsync())
                {
                    receta = new RecetaModel
                    {
                        IdReceta = reader.GetGuid(reader.GetOrdinal("IdReceta")),
                        IdMedico = reader.GetGuid(reader.GetOrdinal("IdMedico")),
                        IdPaciente = reader.GetGuid(reader.GetOrdinal("IdPaciente")),
                        PacPeso = reader.GetDecimal(reader.GetOrdinal("PacPeso")),
                        PacTalla = reader.GetDecimal(reader.GetOrdinal("PacTalla")),
                        PacEmbarazo = reader.GetBoolean(reader.GetOrdinal("PacEmbarazo")),
                        PacSemAmenorrea = reader.IsDBNull(reader.GetOrdinal("PacSemAmenorrea")) ? null : reader.GetInt32(reader.GetOrdinal("PacSemAmenorrea")),
                        PacLactancia = reader.GetBoolean(reader.GetOrdinal("PacLactancia")),
                        PacCreatinina = reader.IsDBNull(reader.GetOrdinal("PacCreatinina")) ? null : reader.GetDecimal(reader.GetOrdinal("PacCreatinina")),
                        FechaUltimaModificacion = reader.GetDateTime(reader.GetOrdinal("FechaUltimaModificacion"))
                    };
                }

                // Leer los detalles de la receta
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        detalles.Add(new DetalleRecetaModel
                        {
                            IdDetalleReceta = reader.GetGuid(reader.GetOrdinal("IdDetalleReceta")),
                            IdReceta = reader.GetGuid(reader.GetOrdinal("IdReceta")),
                            MedicamentoType = reader.GetString(reader.GetOrdinal("MedicamentoType")),
                            MedicamentoId = reader.GetInt32(reader.GetOrdinal("MedicamentoId")),
                            MedicamentoNombre = reader["MedicamentoNombre"]?.ToString(), // El nombre del medicamento según el tipo
                            UnidadDispensacionId = reader.GetInt32(reader.GetOrdinal("UnidadDispensacionId")),
                            UnidadDispensacion = reader["UnidadDispensacion"]?.ToString(), // Nombre de la unidad de dispensación
                            RutaAdministracionId = reader.GetInt32(reader.GetOrdinal("RutaAdministracionId")),
                            RutaAdministracion = reader["RutaAdministracion"]?.ToString(), // Nombre de la ruta de administración
                            CantidadDiaria = reader.GetDecimal(reader.GetOrdinal("CantidadDiaria")),
                            Indicacion = reader["Indicacion"]?.ToString(),
                            Duracion = reader.GetInt32(reader.GetOrdinal("Duracion")),
                            UnidadDuracion = reader["UnidadDuracion"]?.ToString(),
                            PeriodoInicio = reader.GetDateTime(reader.GetOrdinal("PeriodoInicio")),
                            PeriodoTerminacion = reader.IsDBNull(reader.GetOrdinal("PeriodoTerminacion")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("PeriodoTerminacion")),
                            Surtido = reader.GetBoolean(reader.GetOrdinal("Surtido"))
                        });
                    }

                }

                // Leer los datos del paciente
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    paciente = new PacienteModel
                    {
                        IdPaciente = reader.GetGuid(reader.GetOrdinal("IdPaciente")),
                        NombreUsuario = reader.GetString(reader.GetOrdinal("NombreUsuario")),
                        PrimerApellido = reader.GetString(reader.GetOrdinal("PrimerApellido")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Genero = reader.GetString(reader.GetOrdinal("Genero"))
                    };
                }

                // Leer los datos del médico
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    medico = new MedicoModel
                    {
                        IdMedico = reader.GetGuid(reader.GetOrdinal("IdMedico")),
                        Universidad = reader.GetString(reader.GetOrdinal("Universidad")),
                        CedulaGeneral = reader.GetString(reader.GetOrdinal("CedulaGeneral")),
                        Especialidad = reader.GetString(reader.GetOrdinal("Especialidad")),
                        Email = reader.GetString(reader.GetOrdinal("Email"))
                    };
                }

                // Leer los datos del grupo empresarial
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    grupoEmpresarial = new GrupoEmpresarialModel
                    {
                        IdGEMP = reader.GetGuid(reader.GetOrdinal("IdGEMP")),
                        Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                        LogoBase64 = reader.GetString(reader.GetOrdinal("LogoBase64"))
                    };
                }

                return (receta, detalles, paciente, medico, grupoEmpresarial);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la receta: {ex.Message}");
            }
        }

        public async Task SurtirMedicamentosAsync(Guid idReceta, List<Guid> detallesReceta)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("PuntoVenta_SurtirMedicamentos", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Parámetros para el SP
                command.Parameters.AddWithValue("@IdReceta", idReceta);

                // Crear DataTable para los detalles de la receta
                var detalleTable = new DataTable();
                detalleTable.Columns.Add("Id", typeof(Guid));
                foreach (var idDetalle in detallesReceta)
                {
                    detalleTable.Rows.Add(idDetalle);
                }

                var detallesParam = new SqlParameter("@DetallesReceta", SqlDbType.Structured)
                {
                    TypeName = "dbo.GuidList",
                    Value = detalleTable
                };

                command.Parameters.Add(detallesParam);

                // Ejecutar el SP
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al surtir los medicamentos: {ex.Message}");
            }
        }

        public async Task<(PuntoVentaRecetaModel?, List<DetalleRecetaModel>)> ConsultarRecetaPorIdAsync(int idQR)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("PuntoVenta_ConsultarPorId", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Parámetro del primer SP
                command.Parameters.AddWithValue("@Id", idQR);

                PuntoVentaRecetaModel? recetaModel = null;

                // Ejecutar el primer SP
                using (var reader = await command.ExecuteReaderAsync())
                {
                    // Mapeo de la receta principal
                    if (await reader.ReadAsync())
                    {
                        recetaModel = new PuntoVentaRecetaModel
                        {
                            // Información de la receta
                            IdReceta = reader.GetGuid(reader.GetOrdinal("IdReceta")),
                            IdMedico = reader.GetGuid(reader.GetOrdinal("IdMedico")),

                            // Información del médico
                            NombresMedico = reader["NombresMedicos"]?.ToString(),
                            PrimerApellidoMedico = reader["PrimerApellidoMedico"]?.ToString(),
                            SegundoApellidoMedico = reader["SegundoApellidoMedico"]?.ToString(),
                            Movil = reader["Movil"]?.ToString(),
                            Email = reader["Email"]?.ToString(),
                            Universidad = reader["Universidad"]?.ToString(),
                            CedulaGeneral = reader["CedulaGeneral"]?.ToString(),
                            Especialidad = reader["Especialidad"]?.ToString(),
                            CedulaEspecialidad = reader["CedulaEspecialidad"]?.ToString(),
                            Horario = reader["Horario"]?.ToString(),
                            Firma = reader["Firma"]?.ToString(),

                            // Información del paciente
                            IdPaciente = reader.GetGuid(reader.GetOrdinal("IdPaciente")),
                            NombresPaciente = reader["NombresPacientes"]?.ToString(),
                            PrimerApellidoPaciente = reader["PrimerApellidoPacientes"]?.ToString(),
                            SegundoApellidoPaciente = reader["SegundoApellidoPacientes"]?.ToString(),
                            PacPeso = reader.GetDecimal(reader.GetOrdinal("PacPeso")),
                            Genero = reader["Genero"]?.ToString(),
                            PacTalla = reader.GetDecimal(reader.GetOrdinal("PacTalla")),

                            // Información del grupo empresarial
                            IdGEMP = reader.GetGuid(reader.GetOrdinal("IdGEMP")),
                            NombreGrupoEmpresarial = reader["Nombre"]?.ToString(),
                            LogoBase64 = reader["LogoBase64"]?.ToString(),

                            // Información de la sucursal
                            IdSucursal = reader.GetGuid(reader.GetOrdinal("IdSucursal")),
                            NumeroSucursal = reader["Numero"]?.ToString(),
                            NombreSucursal = reader["NombreSucursal"]?.ToString(),
                            RegistroSanitario = reader["RegistroSanitario"]?.ToString(),
                            DomicilioSucursal = reader["Domicilio"]?.ToString(),
                            IdAsentamiento = reader.GetInt32(reader.GetOrdinal("IdAsentamiento")),

                            // Información del asentamiento
                            NombreAsentamiento = reader["NombreAsentamiento"]?.ToString(),
                            IdTipoAsentamiento = reader.GetInt32(reader.GetOrdinal("IdTipoAsentamiento")),
                            TipoAsentamiento = reader["TipoAsentamiento"]?.ToString(),
                            IdCP = reader.GetInt32(reader.GetOrdinal("IdCP")),
                            CodigoPostal = reader["CodigoPostal"]?.ToString(),

                            // Información de la ubicación
                            IdMunicipio = reader.GetInt32(reader.GetOrdinal("IdMunicipio")),
                            NombreMunicipio = reader["NombreMunicipio"]?.ToString(),
                            IdCiudad = reader.GetInt32(reader.GetOrdinal("IdCiudad")),
                            NombreCiudad = reader["NombreCiudad"]?.ToString(),
                            IdEntidad = reader.GetInt32(reader.GetOrdinal("IdEntidad")),
                            Estado = reader["Estado"]?.ToString(),
                            Abreviatura = reader["Abreviatura"]?.ToString()
                        };
                    }
                }

                if (recetaModel == null)
                {
                    return (null, new List<DetalleRecetaModel>()); // No se encontró la receta
                }

                // Segunda llamada al SP para obtener los detalles
                using var commandDetalles = new SqlCommand("PuntoVenta_GetDetalleReceta", connection);
                commandDetalles.CommandType = CommandType.StoredProcedure;

                // Parámetros para el segundo SP
                commandDetalles.Parameters.AddWithValue("@IdReceta", recetaModel.IdReceta);

                var detalles = new List<DetalleRecetaModel>();

                using (var readerDetalles = await commandDetalles.ExecuteReaderAsync())
                {
                    while (await readerDetalles.ReadAsync())
                    {
                        detalles.Add(new DetalleRecetaModel
                        {
                            IdDetalleReceta = readerDetalles.GetGuid(readerDetalles.GetOrdinal("IdDetalleReceta")),
                            IdReceta = readerDetalles.GetGuid(readerDetalles.GetOrdinal("IdReceta")),
                            MedicamentoType = readerDetalles.GetString(readerDetalles.GetOrdinal("MedicamentoType")),
                            MedicamentoId = readerDetalles.GetInt32(readerDetalles.GetOrdinal("MedicamentoId")),
                            MedicamentoNombre = readerDetalles["MedicamentoNombre"]?.ToString(), // Nombre o resumen del medicamento
                            UnidadDispensacionId = readerDetalles.GetInt32(readerDetalles.GetOrdinal("UnidadDispensacionId")),
                            UnidadDispensacion = readerDetalles["UnidadDispensacion"]?.ToString(), // Nombre de la unidad de dispensación
                            RutaAdministracionId = readerDetalles.GetInt32(readerDetalles.GetOrdinal("RutaAdministracionId")),
                            RutaAdministracion = readerDetalles["RutaAdministracion"]?.ToString(), // Nombre de la ruta de administración
                            CantidadDiaria = readerDetalles.GetDecimal(readerDetalles.GetOrdinal("CantidadDiaria")),
                            Indicacion = readerDetalles["Indicacion"]?.ToString(),
                            IndicacionNombre = readerDetalles["IndicacionNombre"]?.ToString(), // Nuevo campo
                            Frecuencia = readerDetalles["Frecuencia"]?.ToString() ?? string.Empty, // Nuevo campo, con valor predeterminado
                            Observaciones = readerDetalles["Observaciones"]?.ToString(), // Nuevo campo
                            Duracion = readerDetalles.GetInt32(readerDetalles.GetOrdinal("Duracion")),
                            UnidadDuracion = readerDetalles["UnidadDuracion"]?.ToString(),
                            PeriodoInicio = readerDetalles.GetDateTime(readerDetalles.GetOrdinal("PeriodoInicio")),
                            PeriodoTerminacion = readerDetalles.IsDBNull(readerDetalles.GetOrdinal("PeriodoTerminacion"))
                            ? (DateTime?)null
                            : readerDetalles.GetDateTime(readerDetalles.GetOrdinal("PeriodoTerminacion")),
                                                Surtido = readerDetalles.GetBoolean(readerDetalles.GetOrdinal("Surtido"))
                        });

                    }

                }

                return (recetaModel, detalles);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al consultar la receta: {ex.Message}");
            }
        }



    }
}
