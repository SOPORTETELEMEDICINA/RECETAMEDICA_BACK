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
        public async Task<UsuarioPaciente> GetPacienteByIdUsuarioAsync(Guid idUsuario)
        {
            var idUsuarioParam = new SqlParameter("@IdUsuario", idUsuario);

            var usuarioPaciente = await _context.UsuarioPacientes
                .FromSqlRaw("EXEC Paciente_GetPacienteByIdUsuario @IdUsuario", idUsuarioParam)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return usuarioPaciente;
        }

        // Este método devuelve un UsuarioPaciente basado en el IdPaciente
        public async Task<UsuarioPaciente> GetPacienteByIdPacienteAsync(Guid idPaciente)
        {
            var idPacienteParam = new SqlParameter("@IdPaciente", idPaciente);

            var usuarioPaciente = await _context.UsuarioPacientes
                .FromSqlRaw("EXEC Paciente_GetPacienteByIdPaciente @IdPaciente", idPacienteParam)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return usuarioPaciente;
        }

        // Este método devuelve una lista de UsuarioPaciente basado en la búsqueda por nombre
        public async Task<IEnumerable<UsuarioPaciente>> GetPacienteByNameAsync(string nombreBusqueda)
        {
            var nombreParam = new SqlParameter("@NombreBusqueda", nombreBusqueda);

            var usuariosPacientes = await _context.UsuarioPacientes
                .FromSqlRaw("EXEC Paciente_GetPacienteByName @NombreBusqueda", nombreParam)
                .AsNoTracking()
                .ToListAsync();

            return usuariosPacientes;
        }

        public async Task<IEnumerable<Paciente>> GetPacientesByEntidadNacimientoAsync(int idEntidadNacimiento)
        {
            return await _context.Pacientes
                .Where(p => p.IdEntidadNacimiento == idEntidadNacimiento)
                .ToListAsync();
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




        public async Task<IEnumerable<EntidadNacimiento>> GetEntidadesFederativasAsync()
        {
            var entidades = await _context.Set<EntidadNacimiento>()
                .FromSqlRaw("EXEC Paciente_GetEntidadesFederativas")
                .AsNoTracking()
                .ToListAsync();

            return entidades;
        }

    }
}
