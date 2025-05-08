namespace RMD.Extensions.System
{
    public static class RoleIds
    {
        public static readonly Guid SuperAdminId = Guid.Parse("7905213C-B0CB-4D42-A997-20094EF41F9C");
        public static readonly Guid MedicoId = Guid.Parse("DE5DFDDC-F6CC-4B7F-B805-286732501E57");
        public static readonly Guid ResponsableFarmaciaId = Guid.Parse("CD5FF082-0BF4-4848-BA0C-38EDA4921954");
        public static readonly Guid PacienteId = Guid.Parse("635A6E50-D6B0-4C0C-BC0A-96B617A21BEE");
        public static readonly Guid EmpleadoFarmaciaId = Guid.Parse("D0E20BC6-C1E8-42C9-B80C-A9C92D55D3E9");
        public static readonly Guid SupervisorSucursalesId = Guid.Parse("68C87CA4-2499-4A9C-B0DC-EEE99B7078BF");
        public static readonly Guid ParticularId = Guid.Parse("635A6E50-D6B0-4C0C-BC0A-96B617A21BEE");
    }

    public static class RoleHelper
    {
        // Diccionario que mapea roles con sus permisos
        private static readonly Dictionary<string, List<string>> RolePermissions = new()
        {
            { "SuperAdmin", new List<string>
                {
                    "GestionarGruposEmpresariales",
                    "GestionarSucursales",
                    "GestionarUsuarios",
                    "LeerActualizarEliminarRecetas",
                    "GestionarCatalogos",
                    "VerReportes",
                    "GestionarEventosDeSalud",
                    "GestionarEventosDeMedicacion",
                    "EditarPerfilDeUsuario"
                }
            },
            { "Medico", new List<string>
                {
                    "GestionarPacientes",
                    "VerCrearCancelarRecetas",
                    "AccederListaDePacientes",
                    "ImprimirEnviarRecetas",
                    "GenerarReportes",
                    "EditarPerfilDeUsuario"
                }
            },
            { "Paciente", new List<string>
                {
                    "VerImprimirRecetas",
                    "CrearEventosDeSalud",
                    "CrearEventosDeMedicacion",
                    "EditarPerfilDeUsuario"
                }
            },
            { "EmpleadoFarmacia", new List<string>
                {
                    "VerImprimirRecetas",
                    "SurtirRecetas",
                    "EditarContraseña"
                }
            },
            { "SupervisorSucursales", new List<string>
                {
                    "GestionarUsuariosDeSucursal",
                    "VerImprimirRecetas",
                    "SurtirRecetas",
                    "GenerarReportes",
                    "EditarPerfilDeUsuario"
                }
            },
            { "Particular", new List<string>
                {
                    "ConsultarCatalogo",
                    "ComprarMedicamentos",
                    "EditarPerfilDeUsuario"
                }
            }
        };

        // Método para obtener el rol del usuario
        public static string GetUserRole(Guid userId)
        {
            if (userId == RoleIds.SuperAdminId) return "SuperAdmin";
            if (userId == RoleIds.MedicoId) return "Medico";
            if (userId == RoleIds.ResponsableFarmaciaId) return "ResponsableFarmacia";
            if (userId == RoleIds.PacienteId) return "Paciente";
            if (userId == RoleIds.EmpleadoFarmaciaId) return "EmpleadoFarmacia";
            if (userId == RoleIds.SupervisorSucursalesId) return "SupervisorSucursales";
            if (userId == RoleIds.ParticularId) return "Particular";

            return "Unknown"; // Si no coincide con ningún ID
        }

        // Método para validar si un rol tiene permiso para una acción
        public static bool HasPermission(Guid userId, string requiredPermission)
        {
            var role = GetUserRole(userId);

            if (RolePermissions.TryGetValue(role, out var permissions))
            {
                return permissions.Contains(requiredPermission);
            }

            return false; // Si no coincide el rol o el permiso no está definido
        }
    }
}
