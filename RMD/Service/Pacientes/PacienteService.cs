using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Extensions;
using RMD.Interface.Pacientes;
using RMD.Models.Pacientes;
using System.Data;

namespace RMD.Service.Pacientes
{
    public class PacienteService : IPacienteService
    {
        private readonly PacientesDbContext _context;

        public PacienteService(PacientesDbContext context)
        {
            _context = context;
        }

        // Este método devuelve un UsuarioPaciente basado en el IdUsuario
        public async Task<PacienteConsultaRequest> GetPacienteByIdUsuarioAsync(Guid idUsuario)
        {
            try
            {
                var idUsuarioParam = new SqlParameter("@IdUsuario", SqlDbType.UniqueIdentifier)
                {
                    Value = idUsuario
                };

                var usuarioPaciente = await _context.UsuarioPacientes
                    .FromSqlRaw("EXEC Paciente_GetPacienteByIdUsuario @IdUsuario", idUsuarioParam)
                    .AsNoTracking()
                    .ToListAsync();

                if (usuarioPaciente.Count > 0)
                {
                    var alergias = await ObtenerAlergiasPorIdsAsync(usuarioPaciente.First().Alergias);
                    var molecules = await ObtenerMoleculesPorIdsAsync(usuarioPaciente.First().Molecules);
                    var patologias = await ObtenerCIM10PorIdsAsync(usuarioPaciente.First().Patologias);

                    var pacienteConsulta = new PacienteConsultaRequest
                    {
                        IdPaciente = usuarioPaciente.First().IdPaciente,
                        IdGEMP = usuarioPaciente.First().IdGEMP,
                        IdSucursal = usuarioPaciente.First().IdSucursal,
                        Nombres = usuarioPaciente.First().Nombres,
                        PrimerApellido = usuarioPaciente.First().PrimerApellido,
                        SegundoApellido = usuarioPaciente.First().SegundoApellido,
                        FechaNacimiento = usuarioPaciente.First().FechaNacimiento,
                        IdEntidadNacimiento = usuarioPaciente.First().IdEntidadNacimiento,
                        Genero = usuarioPaciente.First().Genero,
                        Movil = usuarioPaciente.First().Movil,
                        Email = usuarioPaciente.First().Email,
                        Domicilio = usuarioPaciente.First().Domicilio,
                        Alergias = alergias,
                        Molecules = molecules,
                        Patologias = patologias
                    };

                    return pacienteConsulta;
                }
                else
                {
                    throw new KeyNotFoundException("Paciente no encontrado.");
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Error en la base de datos: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el paciente: {ex.Message}");
            }
        }


        public async Task<PacienteConsultaRequest> GetPacienteByIdPacienteAsync(Guid idPaciente)
        {
            try
            {
                var idPacienteParam = new SqlParameter("@IdPaciente", SqlDbType.UniqueIdentifier)
                {
                    Value = idPaciente
                };

                var usuarioPaciente = await _context.UsuarioPacientes
                    .FromSqlRaw("EXEC Paciente_GetPacienteByIdPaciente @IdPaciente", idPacienteParam)
                    .AsNoTracking()
                    .ToListAsync();

                if (usuarioPaciente.Count > 0)
                {
                    var alergias = await ObtenerAlergiasPorIdsAsync(usuarioPaciente.First().Alergias);
                    var molecules = await ObtenerMoleculesPorIdsAsync(usuarioPaciente.First().Molecules);
                    var patologias = await ObtenerCIM10PorIdsAsync(usuarioPaciente.First().Patologias);

                    var pacienteConsulta = new PacienteConsultaRequest
                    {
                        IdPaciente = usuarioPaciente.First().IdPaciente,
                        IdGEMP = usuarioPaciente.First().IdGEMP,
                        IdSucursal = usuarioPaciente.First().IdSucursal,
                        Nombres = usuarioPaciente.First().Nombres,
                        PrimerApellido = usuarioPaciente.First().PrimerApellido,
                        SegundoApellido = usuarioPaciente.First().SegundoApellido,
                        FechaNacimiento = usuarioPaciente.First().FechaNacimiento,
                        IdEntidadNacimiento = usuarioPaciente.First().IdEntidadNacimiento,
                        Genero = usuarioPaciente.First().Genero,
                        Movil = usuarioPaciente.First().Movil,
                        Email = usuarioPaciente.First().Email,
                        Domicilio = usuarioPaciente.First().Domicilio,
                        Alergias = alergias,
                        Molecules = molecules,
                        Patologias = patologias
                    };

                    return pacienteConsulta;
                }
                else
                {
                    throw new KeyNotFoundException("Paciente no encontrado.");
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Error en la base de datos: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el paciente: {ex.Message}");
            }
        }


        public async Task<IEnumerable<Paciente>> GetPacientesByEntidadNacimientoAsync(int idEntidadNacimiento)
        {
            return await _context.Pacientes
                .Where(p => p.IdEntidadNacimiento == idEntidadNacimiento)
                .ToListAsync();
        }

        // Este método devuelve una lista de UsuarioPaciente basado en la búsqueda por nombre
        public async Task<IEnumerable<PacienteConsultaRequest>> GetPacienteByNameAsync(string nombreBusqueda)
        {
            var nombreParam = new SqlParameter("@NombreBusqueda", nombreBusqueda);

            var usuariosPacientes = await _context.UsuarioPacientes
                .FromSqlRaw("EXEC Paciente_GetPacienteByName @NombreBusqueda", nombreParam)
                .AsNoTracking()
                .ToListAsync();

            var resultado = new List<PacienteConsultaRequest>();

            foreach (var paciente in usuariosPacientes)
            {
                // Enviar las cadenas de IDs directamente al stored procedure
                var alergias = await ObtenerAlergiasPorIdsAsync(paciente.Alergias);
                var molecules = await ObtenerMoleculesPorIdsAsync(paciente.Molecules);
                var patologias = await ObtenerCIM10PorIdsAsync(paciente.Patologias);

                // Crear el objeto PacienteConsultaRequest con los detalles obtenidos
                var pacienteConsulta = new PacienteConsultaRequest
                {
                    IdPaciente = paciente.IdPaciente,
                    IdGEMP = paciente.IdGEMP,
                    IdSucursal = paciente.IdSucursal,
                    Nombres = paciente.Nombres,
                    PrimerApellido = paciente.PrimerApellido,
                    SegundoApellido = paciente.SegundoApellido,
                    FechaNacimiento = paciente.FechaNacimiento,
                    IdEntidadNacimiento = paciente.IdEntidadNacimiento,
                    Genero = paciente.Genero,
                    Movil = paciente.Movil,
                    Email = paciente.Email,
                    Domicilio = paciente.Domicilio,
                    Alergias = alergias,  // Lista de detalles de alergias
                    Molecules = molecules,  // Lista de detalles de moléculas
                    Patologias = patologias  // Lista de detalles de CIM10 (patologías)
                };

                resultado.Add(pacienteConsulta);
            }

            return resultado;
        }      

        public async Task<bool> CreatePacienteAsync(PacienteCreateConListas pacienteRequest, Guid idUsuarioSolicitante)
        {
            // Convertir las listas a strings con delimitadores
            var pacienteConStrings = new PacienteCreate
            {
                IdUsuario = pacienteRequest.IdUsuario,
                FechaNacimiento = pacienteRequest.FechaNacimiento,
                IdEntidadNacimiento = pacienteRequest.IdEntidadNacimiento,
                Genero = pacienteRequest.Genero,
                Alergias = string.Join(";", pacienteRequest.Alergias ?? new List<string>()),
                Molecules = string.Join(";", pacienteRequest.Molecules ?? new List<string>()),
                Patologias = string.Join(";", pacienteRequest.Patologias ?? new List<string>())
            };

            var pacienteTable = new List<PacienteCreate> { pacienteConStrings }.ToDataTable();
            var parameter = new SqlParameter("@PacienteTable", SqlDbType.Structured)
            {
                TypeName = "dbo.CreatePacienteTableType",
                Value = pacienteTable
            };

            var idUsuarioSolicitanteParam = new SqlParameter("@IdUsuarioSolicitante", idUsuarioSolicitante);
            var outputMessageParam = new SqlParameter("@OutputMessage", SqlDbType.NVarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC Pacientes_CrearPaciente @PacienteTable, @IdUsuarioSolicitante, @OutputMessage OUTPUT",
                parameter, idUsuarioSolicitanteParam, outputMessageParam
            );

            var outputMessage = outputMessageParam.Value.ToString();

            return outputMessage.Contains("creado con éxito");
        }

        public async Task<bool> UpdatePacienteAsync(PacienteConListas pacienteRequest, Guid idUsuarioSolicitante)
        {
            // Convertir las listas a strings con delimitadores
            var pacienteConStrings = new Paciente
            {
                IdPaciente = pacienteRequest.IdPaciente, // Asegúrate de que el IdPaciente esté presente en PacienteConListas
                IdUsuario = pacienteRequest.IdUsuario,
                FechaNacimiento = pacienteRequest.FechaNacimiento,
                IdEntidadNacimiento = pacienteRequest.IdEntidadNacimiento,
                Genero = pacienteRequest.Genero,
                Alergias = string.Join(";", pacienteRequest.Alergias ?? new List<string>()),
                Molecules = string.Join(";", pacienteRequest.Molecules ?? new List<string>()),
                Patologias = string.Join(";", pacienteRequest.Patologias ?? new List<string>()),
                IdMedico = pacienteRequest.IdMedico
            };

            // Convertir el modelo a un DataTable
            var pacienteTable = new List<Paciente> { pacienteConStrings }.ToDataTable();
            var parameter = new SqlParameter("@PacienteTable", SqlDbType.Structured)
            {
                TypeName = "dbo.PacienteTableType", // Asegúrate de que el tipo coincida con el que tienes en SQL
                Value = pacienteTable
            };

            var idUsuarioSolicitanteParam = new SqlParameter("@IdUsuarioSolicitante", idUsuarioSolicitante);
            var outputMessageParam = new SqlParameter("@OutputMessage", SqlDbType.NVarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC Pacientes_UpdatePaciente @PacienteTable, @IdUsuarioSolicitante, @OutputMessage OUTPUT",
                parameter, idUsuarioSolicitanteParam, outputMessageParam
            );

            var outputMessage = outputMessageParam.Value.ToString();

            return outputMessage.Contains("actualizado con éxito");
        }

        public async Task<bool> EliminarPacienteAsync(Guid idPaciente, Guid idUsuarioSolicitante)
        {
            var idPacienteParam = new SqlParameter("@IdPaciente", idPaciente);
            var idUsuarioSolicitanteParam = new SqlParameter("@IdUsuarioSolicitante", idUsuarioSolicitante);
            var outputMessageParam = new SqlParameter("@OutputMessage", SqlDbType.NVarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC Pacientes_EliminarPaciente @IdPaciente, @IdUsuarioSolicitante, @OutputMessage OUTPUT",
                idPacienteParam, idUsuarioSolicitanteParam, outputMessageParam
            );

            var outputMessage = outputMessageParam.Value.ToString();

            return outputMessage.Contains("eliminado con éxito");
        }

        public async Task<IEnumerable<EntidadNacimiento>> GetEntidadesFederativasAsync()
        {
            var entidades = await _context.Set<EntidadNacimiento>()
                .FromSqlRaw("EXEC Paciente_GetEntidadesFederativas")
                .AsNoTracking()
                .ToListAsync();

            return entidades;
        }

        public async Task<IEnumerable<PacienteConsultaRequest>> GetPacientesBySucursalAsync(Guid idSucursal)
        {
            var idSucursalParam = new SqlParameter("@IdSucursal", idSucursal);

            var usuariosPacientes = await _context.UsuarioPacientes
                .FromSqlRaw("EXEC Paciente_GetPacientesBySucursal @IdSucursal", idSucursalParam)
                .AsNoTracking()
                .ToListAsync();

            var resultado = new List<PacienteConsultaRequest>();

            foreach (var paciente in usuariosPacientes)
            {
                var alergiasIds = paciente.Alergias;
                var moleculesIds = paciente.Molecules;
                var patologiasIds = paciente.Patologias;

                var alergias = await ObtenerAlergiasPorIdsAsync(alergiasIds);
                var molecules = await ObtenerMoleculesPorIdsAsync(moleculesIds);
                var patologias = await ObtenerCIM10PorIdsAsync(patologiasIds);

                var pacienteConsulta = new PacienteConsultaRequest
                {
                    IdPaciente = paciente.IdPaciente,
                    IdGEMP = paciente.IdGEMP,
                    IdSucursal = paciente.IdSucursal,
                    Nombres = paciente.Nombres,
                    PrimerApellido = paciente.PrimerApellido,
                    SegundoApellido = paciente.SegundoApellido,
                    FechaNacimiento = paciente.FechaNacimiento,
                    IdEntidadNacimiento = paciente.IdEntidadNacimiento,
                    Genero = paciente.Genero,
                    Movil = paciente.Movil,
                    Email = paciente.Email,
                    Domicilio = paciente.Domicilio,
                    Alergias = alergias,
                    Molecules = molecules,
                    Patologias = patologias
                };

                resultado.Add(pacienteConsulta);
            }

            return resultado;
        }

        public async Task<IEnumerable<PacienteConsultaRequest>> GetPacientesByGEMPAsync(Guid idGEMP)
        {
            var idGEMPParam = new SqlParameter("@IdGEMP", idGEMP);

            var usuariosPacientes = await _context.UsuarioPacientes
                .FromSqlRaw("EXEC Paciente_GetPacientesByGEMP @IdGEMP", idGEMPParam)
                .AsNoTracking()
                .ToListAsync();

            var resultado = new List<PacienteConsultaRequest>();

            foreach (var paciente in usuariosPacientes)
            {
                var alergiasIds = paciente.Alergias;
                var moleculesIds = paciente.Molecules;
                var patologiasIds = paciente.Patologias;

                var alergias = await ObtenerAlergiasPorIdsAsync(alergiasIds);
                var molecules = await ObtenerMoleculesPorIdsAsync(moleculesIds);
                var patologias = await ObtenerCIM10PorIdsAsync(patologiasIds);

                var pacienteConsulta = new PacienteConsultaRequest
                {
                    IdPaciente = paciente.IdPaciente,
                    IdGEMP = paciente.IdGEMP,
                    IdSucursal = paciente.IdSucursal,
                    Nombres = paciente.Nombres,
                    PrimerApellido = paciente.PrimerApellido,
                    SegundoApellido = paciente.SegundoApellido,
                    FechaNacimiento = paciente.FechaNacimiento,
                    IdEntidadNacimiento = paciente.IdEntidadNacimiento,
                    Genero = paciente.Genero,
                    Movil = paciente.Movil,
                    Email = paciente.Email,
                    Domicilio = paciente.Domicilio,
                    Alergias = alergias,
                    Molecules = molecules,
                    Patologias = patologias
                };

                resultado.Add(pacienteConsulta);
            }

            return resultado;
        }

        public async Task<IEnumerable<PacienteConsultaRequest>> GetPacientesByMedicoAsync(Guid idMedico)
        {
            var idMedicoParam = new SqlParameter("@IdMedico", idMedico);

            var usuariosPacientes = await _context.UsuarioPacientes
                .FromSqlRaw("EXEC Paciente_GetPacientesByMedico @IdMedico", idMedicoParam)
                .AsNoTracking()
                .ToListAsync();

            var resultado = new List<PacienteConsultaRequest>();

            foreach (var paciente in usuariosPacientes)
            {
                var alergiasIds = paciente.Alergias;
                var moleculesIds = paciente.Molecules;
                var patologiasIds = paciente.Patologias;

                var alergias = await ObtenerAlergiasPorIdsAsync(alergiasIds);
                var molecules = await ObtenerMoleculesPorIdsAsync(moleculesIds);
                var patologias = await ObtenerCIM10PorIdsAsync(patologiasIds);

                var pacienteConsulta = new PacienteConsultaRequest
                {
                    IdPaciente = paciente.IdPaciente,
                    IdGEMP = paciente.IdGEMP,
                    IdSucursal = paciente.IdSucursal,
                    Nombres = paciente.Nombres,
                    PrimerApellido = paciente.PrimerApellido,
                    SegundoApellido = paciente.SegundoApellido,
                    FechaNacimiento = paciente.FechaNacimiento,
                    IdEntidadNacimiento = paciente.IdEntidadNacimiento,
                    Genero = paciente.Genero,
                    Movil = paciente.Movil,
                    Email = paciente.Email,
                    Domicilio = paciente.Domicilio,
                    Alergias = alergias,
                    Molecules = molecules,
                    Patologias = patologias
                };

                resultado.Add(pacienteConsulta);
            }

            return resultado;
        }



        private async Task<List<AllergyModel>> ObtenerAlergiasPorIdsAsync(string alergiasIds)
        {
            if (string.IsNullOrEmpty(alergiasIds))
            {
                // Si alergiasIds está vacío o es null, devolvemos una lista vacía.
                return new List<AllergyModel>();
            }

            var alergiasParam = new SqlParameter("@Ids", alergiasIds);

            return await _context.AllergyModels
                .FromSqlRaw("EXEC Vidal_GetAllergiesByIds @Ids", alergiasParam)
                .ToListAsync();
        }

        private async Task<List<MoleculeModel>> ObtenerMoleculesPorIdsAsync(string moleculesIds)
        {
            if (string.IsNullOrEmpty(moleculesIds))
            {
                // Si moleculesIds está vacío o es null, devolvemos una lista vacía.
                return new List<MoleculeModel>();
            }

            var moleculesParam = new SqlParameter("@Ids", moleculesIds);

            return await _context.MoleculeModels
                .FromSqlRaw("EXEC Vidal_GetMoleculesByIds @Ids", moleculesParam)
                .ToListAsync();
        }

        private async Task<List<CIM10Model>> ObtenerCIM10PorIdsAsync(string cim10Ids)
        {
            if (string.IsNullOrEmpty(cim10Ids))
            {
                // Si cim10Ids está vacío o es null, devolvemos una lista vacía.
                return new List<CIM10Model>();
            }

            var cim10Param = new SqlParameter("@Ids", cim10Ids);

            return await _context.CIM10Models
                .FromSqlRaw("EXEC Vidal_GetCIM10ByIds @Ids", cim10Param)
                .ToListAsync();
        }

    }
}
