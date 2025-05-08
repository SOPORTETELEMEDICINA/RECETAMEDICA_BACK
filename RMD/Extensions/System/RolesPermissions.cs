namespace RMD.Extensions.System
{
    public static class RolesPermissions
    {

        public static class SucursalesController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosSucursalesController =
            {
                 "Super Admin",
                "Supervisor Sucursales",
                "Particular",
                "Medico",
                "Paciente",
                "Responsable Farmacia",
                "Empleado Farmacia"
            };

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesSucursalesController = new()
            {
                { "CreateSucursal", new[] { "Super Admin", "Supervisor Sucursales", "Particular" } },
                { "UpdateSucursal", new[] { "Super Admin", "Supervisor Sucursales", "Particular" } },
                { "DeleteSucursal", new[] { "Super Admin" } },
                { "GetSucursalById", new[] { "Super Admin", "Supervisor Sucursales", "Particular", "Medico", "Pacientem", "Responsable Farmacia", "Empleado Farmacia" } },
                { "GetSucursalesByGEMP", new[] { "Super Admin", "Supervisor Sucursales", "Particular", "Medico", "Pacientem", "Responsable Farmacia", "Empleado Farmacia" } },
                { "GetSucursalesByGEMPAndAsentamiento", new[] { "Super Admin", "Supervisor Sucursales", "Particular" } }
            };
        }

        public static class CatGrupoEmpresarialController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosCatGrupoEmpresarialController =
            {
                "Super Admin",
                "Supervisor ucursales",
                "Particular",
                "Medico",
                "Paciente",
                "Responsable Farmacia",
                "Empleado Farmacia"
            };

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesCatGrupoEmpresarialController = new()
            {
                { "GetAllGrupoEmpresarial", new[] { "Super Admin", "Supervisor Sucursales", "Particular", "Medico", "Paciente", "Responsable Farmacia", "Empleado Farmacia" } },
                { "GetGrupoEmpresarialById", new[] { "Super Admin", "Supervisor Sucursales", "Particular", "Medico", "Paciente", "Responsable Farmacia", "Empleado Farmacia" } },
                { "CreateGrupoEmpresarial", new[] { "Super Admin" } },
                { "UpdateGrupoEmpresarial", new[] { "Super Admin" } }
            };
        }
        public static class TipoUsuarioController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosTipoUsuarioController =
            {
                "Super Admin",
                "Supervisor Sucursales",
                "Responsable Farmacia"
            };

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesTipoUsuarioController = new()
            {
                { "GetAllTipoUsuario", new[] { "Super Admin", "Supervisor Sucursales", "Responsable Farmacia" } },
                { "GetTipoUsuarioById", new[] { "Super Admin", "Supervisor Sucursales", "Responsable Farmacia" } },
                { "CreateTipoUsuario", new[] { "Super Admin" } },
                { "UpdateTipoUsuario", new[] { "Super Admin" } }
            };
        }
        // Roles para MedicosController
        public static class MedicosController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosMedicosController =
            {
                "Super Admin",
                "Medico",
                "Supervisor Sucursales",
                "Responsable Farmacia",
                "Particular"
            };

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesMedicosController = new()
            {
                { "GetMedicoByIdMedico", new[] { "Super Admin", "Medico", "Supervisor Sucursales", "Responsable Farmacia", "Particular" } },
                { "GetMedicosBySucursal", new[] { "Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Particular" } },
                { "GetMedicosByGEMP", new[] { "Super Admin", "Supervisor Sucursales", "Particular" } },
                { "GetMedicoByIdUsuario", new[] { "Super Admin", "Medico", "Supervisor Sucursales", "Particular", "Responsable Farmacia" } },
                { "GetMedicoByName", new[] { "Super Admin", "Medico", "Supervisor Sucursales", "Particular" } },
                { "CreateMedico", new[] { "Super Admin", "Supervisor Sucursales", "Responsable Farmacia" } },
                { "UpdateMedico", new[] { "Super Admin" } },
                { "DeleteMedico", new[] { "Super Admin" } },
                { "GetPacientesBySucursal", new[] { "Super Admin", "Medico", "Particular" } }
            };
        }

        public static class UsuariosController
        {
            public static readonly string[] RolesPermitidosUsuariosController =
            {
                "Super Admin",
                "Supervisor Sucursales",
                "Responsable Farmacia",
                "Empleado Farmacia",
                "Medico",
                "Paciente"
            };

            public static readonly Dictionary<string, string[]> EndpointRolesUsuariosController = new()
            {
                { "CrearUsuario", new[] { "Super Admin", "Supervisor Sucursales", "Medico"} },
                { "UpdateUsuario", new[] { "Super Admin", "Supervisor Sucursales", "Medico" } },
                { "GetUsuariosByGEMP", new[] { "Super Admin", "Supervisor Sucursales", "Particular", "Medico" } },
                { "GetUsuariosBySucursal", new[] { "Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular" } },
                { "ObtenerUsuarios", new[] { "Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular" } },
                { "EliminarUsuario", new[] { "Super Admin" } },
                { "CambiarPassword", new[] { "Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Empleado Farmacia", "Medico", "Paciente", "Particular" } },
                { "CrearActualizarImagenFirma", new[] { "Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Empleado Farmacia", "Medico", "Paciente", "Particular" } },
                { "ObtenerFirma", new[] { "Super Admin", "Supervisor Sucursales", "Medico", "Particular" } },
                { "EliminarFirma", new[] { "Super Admin"} },
                { "ObtenerImagen", new[] { "Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Empleado Farmacia", "Medico", "Paciente", "Particular" } },
                { "EliminarImagen", new[] { "Super Admin" } }
            };
        }

        public static class PacientesController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosPacientesController =
            {
                "Super Admin",
                "Medico",
                "Particular",
                "Paciente"
            };

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesPacientesController = new()
            {
                { "GetPacienteByIdUsuario", new[] { "Super Admin", "Medico", "Paciente", "Particular" } },
                { "GetPacienteByIdPaciente", new[] { "Super Admin", "Medico", "Particular" } },
                { "GetPacienteByName", new[] { "Super Admin", "Medico", "Particular" } },
                { "GetPacientesByEntidadNacimiento", new[] { "Super Admin", "Medico" } },
                { "GetPacientesBySucursal", new[] { "Super Admin", "Medico", "Particular" } },
                { "GetPacientesByGEMP", new[] { "Super Admin", "Medico", "Particular" } },
                { "GetPacientesByMedico", new[] { "Super Admin", "Medico" , "Particular" } },
                { "CreatePaciente", new[] { "Super Admin", "Medico", "Particular" } },
                { "UpdatePaciente", new[] { "Super Admin", "Medico", "Particular" } },
                { "GetEntidadesFederativas", new[] { "Super Admin", "Medico", "Particular" } },
                { "EliminarPaciente", new[] { "Super Admin" } },
                { "GetEventosSaludByPaciente", new[] { "Paciente" } },
                { "GetEventoSaludById", new[] { "Paciente" } },
                { "CreateEventoSalud", new[] { "Paciente" } },
                { "UpdateEventoSalud", new[] { "Paciente" } },
                { "DeleteEventoSalud", new[] { "Paciente" } }
            };
        }



        public static class ConsultaController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosConsultaController =
            {
                "Super Admin",
                "Medico",
                "Particular",
                "Supervisor Sucursales",
                "Responsable Farmacia"
            };

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesConsultaController = new()
            {
                { "GetAllergiesByName", new[] { "Super Admin", "Medico",  "Particular" } },
                { "GetMoleculeByName", new[] { "Super Admin", "Medico", "Particular" } },
                { "GetCIM10ByName", new[] { "Super Admin", "Medico", "Particular" } },
                { "GetIdsFromLink", new[]  { "Super Admin", "Medico", "Particular" } },
                { "AnalyzePrescription", new[] { "Super Admin", "Medico", "Particular" } },
                { "AnalyzePrescriptionXML", new[] { "Super Admin", "Medico", "Particular" } },
                //{ "AsentamientoByNames", new[] { "Super Admin", "Medico" , "Particular", "Supervisor Sucursales", "Responsable Farmacia" } },
                { "GetTieneEventoSalud", new[] { "Super Admin", "Medico" , "Particular", "Supervisor Sucursales", "Responsable Farmacia" } },
                { "GetTieneEventoMedicamentoso", new[] { "Super Admin", "Medico" , "Particular", "Supervisor Sucursales", "Responsable Farmacia" } },
                { "GetEventosSaludByPaciente", new[] { "Super Admin", "Medico" , "Particular", "Supervisor Sucursales", "Responsable Farmacia" } },

                
                { "SearchPacienteByName", new[] { "Super Admin", "Medico", "Particular" } },

                { "GetMedicamentoByName", new[] { "Super Admin", "Medico", "Particular" } },
                { "RegistrarReceta", new[] { "Super Admin", "Medico", "Particular" } },
                { "ObtenerIdMedicoPorUsuario", new[] { "Super Admin", "Medico", "Particular" } },
                { "ObtenerRecetaPorId", new[] { "Super Admin", "Medico", "Particular" } },
                { "EliminarReceta", new[] { "Super Admin", "Medico", "Particular" } },
                { "ConsultarReceta", new[] { "Super Admin", "Medico", "Particular" } },
                { "ObtenerRecetasPorMedico", new[] { "Super Admin", "Medico", "Particular" } },
                { "ObtenerSucursalesPorUsuario", new[] { "Super Admin", "Medico", "Particular" } },
                { "GetReaccionMedicamentoPrevio", new[] { "Super Admin", "Medico", "Particular" } },
                { "SearchAsentamiento", new[] {"Super Admin", "Medico" , "Particular", "Supervisor Sucursales", "Responsable Farmacia" } },
                { "ObtenerRecetasByIdMedico", new[] { "Super Admin", "Supervisor Sucursales", "Responsable Farmacia" } }
                
            };
        }

        public static class RecetaController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosRecetaController =
            {
                "Super Admin",
                "Medico",
                "Particular",
                "Supervisor Sucursales",
                "Responsable Farmacia",
                "Paciente"
            };

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesRecetaController = new()
            {
      
                { "GetFilteredRecetas", new[] { "Super Admin", "Medico" , "Particular", "Supervisor Sucursales", "Responsable Farmacia" } },
                { "GetFilteredRecetasByIdMedico", new[] {  "Medico" , "Particular" } },
                { "GetRecetaByIdReceta", new[] { "Super Admin", "Medico" , "Particular", "Supervisor Sucursales", "Responsable Farmacia" } },
                { "GetRecetasByIdPaciente", new[] { "Super Admin", "Medico" , "Particular", "Supervisor Sucursales", "Responsable Farmacia", "Paciente" } },
                 { "GetQRByIdReceta", new[] { "Paciente" } },
                


                { "SearchPacienteByName", new[] { "Super Admin", "Medico", "Particular" } },

                { "MedicamentoByName", new[] { "Super Admin", "Medico", "Particular" } },
                { "RegistrarReceta", new[] { "Super Admin", "Medico", "Particular" } },
                { "ObtenerIdMedicoPorUsuario", new[] { "Super Admin", "Medico", "Particular" } },
                { "ObtenerRecetaPorId", new[] { "Super Admin", "Medico", "Particular" } },
                { "EliminarReceta", new[] { "Super Admin", "Medico", "Particular" } },
                { "ConsultarReceta", new[] { "Super Admin", "Medico", "Particular" } },
                { "ObtenerRecetasPorMedico", new[] { "Super Admin", "Medico", "Particular" } },
                { "ObtenerSucursalesPorUsuario", new[] { "Super Admin", "Medico", "Particular" } },
                { "GetReaccionMedicamentoPrevio", new[] { "Super Admin", "Medico", "Particular" } }

            };
        }


        public static class PuntoVentaController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosPuntoVentaController =
            {
                "Super Admin",
                "Supervisor Sucursales",
                "Responsable Farmacia",
                "Empleado Farmacia"
            };

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesPuntoVentaController = new()
            {

                { "ConsultarRecetaPorId", new[] { "Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Empleado Farmacia" } },
                { "SurtirMedicamentos", new[] { "Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Empleado Farmacia" } },
                { "BuscarReceta", new[] { "Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Empleado Farmacia" } }
            };
        }


        public static class DashBoardController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosDashBoardController =
            {
                "Super Admin",
                "Supervisor Sucursales",
                "Responsable Farmacia",
                "Medico",
                "Particular"
            };

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesDashBoardController = new()
            {

                { "GetKpiPacientesRecetas", new[] { "Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular" } },
                { "GetSucursalPacientes", new[] { "Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular"} }
            };
        }
    }
}
