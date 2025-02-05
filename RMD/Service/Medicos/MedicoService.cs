using RMD.Data;
using RMD.Extensions;
using RMD.Interface.Medicos;
using RMD.Models.Medicos;
using System.Xml;
using System.Xml.Linq;

namespace RMD.Service.Medicos
{
    public class MedicoService(MedicosDbContext context, HttpClient httpClient, IConfiguration configuration) : IMedicoService
    {
        private readonly MedicosDbContext _context = context;
        private readonly HttpClient _httpClient = httpClient;
        private readonly string _baseUrl = configuration["VidalApi:BaseUrl"] ?? throw new ArgumentNullException(nameof(_baseUrl));
        private readonly string _appId = configuration["VidalApi:AppId"] ?? throw new ArgumentNullException(nameof(_appId));
        private readonly string _appKey = configuration["VidalApi:AppKey"] ?? throw new ArgumentNullException(nameof(_appKey));


        public async Task<MedicoConsultaRequest> GetMedicoByIdUsuarioAsync(Guid idUsuario)
        {
            var idUsuarioParam = new SqlParameter("@IdUsuario", idUsuario);

            var result = await _context.MedicoConsultaRequest
                .FromSqlRaw("EXEC Medicos_GetMedicoByIdUsuario @IdUsuario", idUsuarioParam)
                .ToListAsync(); // Ejecuta la consulta sin intentar componerla

            return result.FirstOrDefault(); // Retorna el primer resultado
        }

        public async Task<MedicoConsultaRequest> GetMedicoByIdMedicoAsync(Guid idMedico)
        {
            var idMedicoParam = new SqlParameter("@IdMedico", idMedico);

            var medico = await _context.MedicoConsultaRequest
                .FromSqlRaw("EXEC Medicos_GetMedicoByIdMedico @IdMedico", idMedicoParam)
                .ToListAsync(); // Ejecuta la consulta

            return medico.FirstOrDefault(); // Retorna el primer resultado
        }

        public async Task<IEnumerable<MedicoConsultaRequest>> GetMedicosBySucursalAsync(Guid idSucursal)
        {
            var idSucursalParam = new SqlParameter("@IdSucursal", idSucursal);

            var medicos = await _context.MedicoConsultaRequest
                .FromSqlRaw("EXEC Medicos_GetMedicosBySucursal @IdSucursal", idSucursalParam)
                .ToListAsync(); // Ejecuta la consulta y retorna la lista completa

            return medicos;
        }

        public async Task<IEnumerable<MedicoConsultaRequest>> GetMedicosByGEMPAsync(Guid idGEMP)
        {
            var idGEMPParam = new SqlParameter("@IdGEMP", idGEMP);

            var medicos = await _context.MedicoConsultaRequest
                .FromSqlRaw("EXEC Medicos_GetMedicosByGEMP @IdGEMP", idGEMPParam)
                .ToListAsync(); // Ejecuta la consulta y retorna la lista completa

            return medicos;
        }

        public async Task<IEnumerable<MedicoConsultaRequest>> GetMedicoByNameAsync(string nombreBusqueda)
        {
            var nombreParam = new SqlParameter("@NombreBusqueda", nombreBusqueda);

            var result = await _context.MedicoConsultaRequest
                .FromSqlRaw("EXEC Medicos_GetMedicoByName @NombreBusqueda", nombreParam)
                .ToListAsync(); // Ejecuta la consulta y evita composiciones

            return result;
        }

        public async Task<bool> CreateMedicoAsync(MedicoCreate medico, Guid idRol)
        {
            try
            {
                var medicoTable = new List<MedicoCreate> { medico }.ToDataTable(); // Convierte la lista a DataTable

                var medicoParam = new SqlParameter("@MedicoTable", SqlDbType.Structured)
                {
                    TypeName = "dbo.MedicoCreateTableType",
                    Value = medicoTable
                };

                var idRolParam = new SqlParameter("@IdRol", idRol);

                var outputMessageParam = new SqlParameter("@OutputMessage", SqlDbType.NVarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };

                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC Medicos_CreateMedico @MedicoTable, @IdRol, @OutputMessage OUTPUT",
                    medicoParam, idRolParam, outputMessageParam
                );

                var outputMessage = outputMessageParam.Value.ToString();
                Console.WriteLine($"Resultado del SP: {outputMessage}");

                return rowsAffected > 0 || outputMessage.Contains("éxito");
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error SQL: {sqlEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general: {ex.Message}");
                return false;
            }
        }



        public async Task<string> UpdateMedicoAsync(Medico medico, Guid idUsuarioSolicitante)
        {
            var medicoTable = new List<Medico> { medico }.ToDataTable();

            var medicoParam = new SqlParameter("@MedicoTable", SqlDbType.Structured)
            {
                TypeName = "dbo.MedicoTableType",
                Value = medicoTable
            };

            var idUsuarioSolicitanteParam = new SqlParameter("@IdUsuarioSolicitante", idUsuarioSolicitante);

            var outputMessageParam = new SqlParameter("@OutputMessage", SqlDbType.NVarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC Medicos_UpdateMedico @MedicoTable, @IdUsuarioSolicitante, @OutputMessage OUTPUT",
                medicoParam, idUsuarioSolicitanteParam, outputMessageParam
            );

            return outputMessageParam.Value?.ToString();
        }



        public async Task<bool> DeleteMedicoAsync(Guid idMedico, Guid idUsuarioSolicitante)
        {
            var idMedicoParam = new SqlParameter("@IdMedico", idMedico);
            var idUsuarioSolicitanteParam = new SqlParameter("@IdUsuarioSolicitante", idUsuarioSolicitante);

            var outputMessageParam = new SqlParameter("@OutputMessage", SqlDbType.NVarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                "EXEC Medicos_DeleteMedico @IdMedico, @IdUsuarioSolicitante, @OutputMessage OUTPUT",
                idMedicoParam, idUsuarioSolicitanteParam, outputMessageParam
            );

            var outputMessage = outputMessageParam.Value.ToString();
            Console.WriteLine($"Resultado del SP: {outputMessage}");

            return rowsAffected > 0 || outputMessage.Contains("éxito");
        }



        public async Task<IEnumerable<PacientePorSucursalListModel>> GetPacientesBySucursalListAsync(Guid idUsuario)
        {
            var idUsuarioParam = new SqlParameter("@IdUsuario", idUsuario);

            var pacientes = await _context.PacientesPorSucursal
                .FromSqlRaw("EXEC Medicos_GetPacientesBySucursal @IdUsuario", idUsuarioParam)
                .ToListAsync();

            var resultados = new List<PacientePorSucursalListModel>();

            foreach (var p in pacientes)
            {
                var patologias = new List<PatologiaModel>();

                // Verificamos si el string de patologías contiene el prefijo
                if (p.Patologias.Contains("vidal://cim10/code/"))
                {
                    // Procesamos cada código que comienza con el prefijo correcto
                    var codigos = p.Patologias.Split(',')
                        .Select(code => code.Trim())
                        .Where(code => code.StartsWith("vidal://cim10/code/")) // Filtramos solo los códigos válidos
                        .ToList();

                    // Creamos los modelos de patologías
                    foreach (var codigo in codigos)
                    {
                        var patologia = new PatologiaModel
                        {
                            VidalId = codigo,
                            VidalName = await GetVidalNameAsync(codigo.Substring(codigo.LastIndexOf('/') + 1)) // Obtenemos solo el código
                        };

                        patologias.Add(patologia);
                    }
                }

                resultados.Add(new PacientePorSucursalListModel
                {
                    IdUsuario = p.IdUsuario,
                    IdPaciente = p.IdPaciente,
                    IdGEMP = p.IdGEMP,
                    LogoGEMP = p.LogoGEMP,
                    IdSucursal = p.IdSucursal,
                    Nombres = p.Nombres,
                    PrimerApellido = p.PrimerApellido,
                    SegundoApellido = p.SegundoApellido,
                    FechaNacimiento = p.FechaNacimiento,
                    Edad = p.Edad,
                    IdEntidadNacimiento = p.IdEntidadNacimiento,
                    Genero = p.Genero,
                    Alergias = string.IsNullOrEmpty(p.Alergias) ? new List<string>() : p.Alergias.Split(',').ToList(), // Manejo de nulos o vacíos
                    Molecules = string.IsNullOrEmpty(p.Molecules) ? new List<string>() : p.Molecules.Split(',').ToList(), // Manejo de nulos o vacíos
                    Patologias = patologias, // Asignamos la lista de patologías ya procesada
                    Movil = p.Movil,
                    Email = p.Email,
                    Domicilio = p.Domicilio,
                    IdAsentamiento = p.IdAsentamiento,
                    Asentamiento = p.Asentamiento,
                    IdTipoAsentamiento = p.IdTipoAsentamiento,
                    TipoAsentamiento = p.TipoAsentamiento,
                    IdCP = p.IdCP,
                    CodigoPostal = p.CodigoPostal,
                    IdMunicipio = p.IdMunicipio,
                    NoMunicipio = p.NoMunicipio,
                    Municipio = p.Municipio,
                    IdCiudad = p.IdCiudad,
                    Ciudad = p.Ciudad,
                    IdEntidad = p.IdEntidad,
                    Estado = p.Estado,
                    Abreviatura = p.Abreviatura,
                    Status = p.Status
                });

            }

            return resultados;
        }

        private async Task<string> GetVidalNameAsync(string code)
        {
            try
            {
                var apiUrl = $"{_baseUrl}/pathologies?filter=CIM10&code={code}&app_id={_appId}&app_key={_appKey}";

                // Lógica para hacer la llamada HTTP y obtener el XML
                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                var xmlContent = await response.Content.ReadAsStringAsync();
                var vidalName = ParseVidalNameFromXml(xmlContent); // Implementa este método para obtener el vidalName del XML

                return vidalName;
            }
            catch (HttpRequestException httpEx)
            {
                // Manejo de errores de HTTP
                Console.WriteLine($"Error en la solicitud HTTP: {httpEx.Message}");
                throw; // Re-lanzar la excepción si es necesario
            }
            catch (XmlException xmlEx)
            {
                // Manejo de errores de XML
                Console.WriteLine($"Error al parsear el XML: {xmlEx.Message}");
                throw; // Re-lanzar la excepción si es necesario
            }
            catch (Exception ex)
            {
                // Manejo de errores generales
                Console.WriteLine($"Error inesperado: {ex.Message}");
                throw; // Re-lanzar la excepción si es necesario
            }
        }

        private string ParseVidalNameFromXml(string xmlContent)
        {
            try
            {
                // Lógica para parsear el XML y obtener el <vidal:name>
                var document = XDocument.Parse(xmlContent);
                return document.Descendants(XName.Get("name", "http://api.vidal.net/-/spec/vidal-api/1.0/"))
                               .Select(x => x.Value)
                               .FirstOrDefault();
            }
            catch (XmlException xmlEx)
            {
                // Manejo de errores de XML
                Console.WriteLine($"Error al parsear el XML: {xmlEx.Message}");
                throw; // Re-lanzar la excepción si es necesario
            }
            catch (Exception ex)
            {
                // Manejo de errores generales
                Console.WriteLine($"Error inesperado al parsear el XML: {ex.Message}");
                throw; // Re-lanzar la excepción si es necesario
            }
        }




    }
}
