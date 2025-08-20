namespace RMD.Extensions.System
{
    public static class RolesPermissions
    {

        public static class SucursalesController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosSucursalesController =
            [
                "Super Admin",
                "Supervisor Sucursales",
                "Particular",
                "Medico",
                "Paciente",
                "Responsable Farmacia",
                "Empleado Farmacia"
            ];

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesSucursalesController = new()
            {
                { "CreateSucursal", ["Super Admin", "Supervisor Sucursales", "Particular"] },
                { "UpdateSucursal", ["Super Admin", "Supervisor Sucursales", "Particular"] },
                { "DeleteSucursal", ["Super Admin"] },
                { "GetSucursalById", ["Super Admin", "Supervisor Sucursales", "Particular", "Medico", "Pacientem", "Responsable Farmacia", "Empleado Farmacia"] },
                { "GetSucursalesByGEMP", ["Super Admin", "Supervisor Sucursales", "Particular", "Medico", "Pacientem", "Responsable Farmacia", "Empleado Farmacia"] },
                { "GetSucursalesByGEMPAndAsentamiento", ["Super Admin", "Supervisor Sucursales", "Particular"] }
            };
        }

        public static class CatGrupoEmpresarialController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosCatGrupoEmpresarialController =
            [
                "Super Admin",
                "Supervisor ucursales",
                "Particular",
                "Medico",
                "Paciente",
                "Responsable Farmacia",
                "Empleado Farmacia"
            ];

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesCatGrupoEmpresarialController = new()
            {
                { "GetAllGrupoEmpresarial", ["Super Admin", "Supervisor Sucursales", "Particular", "Medico", "Paciente", "Responsable Farmacia", "Empleado Farmacia"] },
                { "GetGrupoEmpresarialById", ["Super Admin", "Supervisor Sucursales", "Particular", "Medico", "Paciente", "Responsable Farmacia", "Empleado Farmacia"] },
                { "CreateGrupoEmpresarial", ["Super Admin"] },
                { "UpdateGrupoEmpresarial", ["Super Admin"] }
            };
        }
        public static class TipoUsuarioController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosTipoUsuarioController =
            [
                "Super Admin",
                "Supervisor Sucursales",
                "Responsable Farmacia"
            ];

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesTipoUsuarioController = new()
            {
                { "GetAllTipoUsuario", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia"] },
                { "GetTipoUsuarioById", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia"] },
                { "CreateTipoUsuario", ["Super Admin"] },
                { "UpdateTipoUsuario", ["Super Admin"] }
            };
        }
        // Roles para MedicosController
        public static class MedicosController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosMedicosController =
            [
                "Super Admin",
                "Medico",
                "Supervisor Sucursales",
                "Responsable Farmacia",
                "Particular"
            ];

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesMedicosController = new()
            {
                { "GetMedicoByIdMedico", ["Super Admin", "Medico", "Supervisor Sucursales", "Responsable Farmacia", "Particular"] },
                { "GetMedicosBySucursal", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Particular"] },
                { "GetMedicosByGEMP", ["Super Admin", "Supervisor Sucursales", "Particular"] },
                { "GetMedicoByIdUsuario", ["Super Admin", "Medico", "Supervisor Sucursales", "Particular", "Responsable Farmacia"] },
                { "GetMedicoByName", ["Super Admin", "Medico", "Supervisor Sucursales", "Particular"] },
                { "CreateMedico", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia"] },
                { "UpdateMedico", ["Super Admin"] },
                { "DeleteMedico", ["Super Admin"] },
                { "GetPacientesBySucursal", ["Super Admin", "Medico", "Particular"] }
            };
        }

        public static class UsuariosController
        {
            public static readonly string[] RolesPermitidosUsuariosController =
            [
                "Super Admin",
                "Supervisor Sucursales",
                "Responsable Farmacia",
                "Empleado Farmacia",
                "Medico",
                "Paciente"
            ];

            public static readonly Dictionary<string, string[]> EndpointRolesUsuariosController = new()
            {
                { "CrearUsuario", ["Super Admin", "Supervisor Sucursales", "Medico"] },
                { "UpdateUsuario", ["Super Admin", "Supervisor Sucursales", "Medico"] },
                { "GetUsuariosByGEMP", ["Super Admin", "Supervisor Sucursales", "Particular", "Medico"] },
                { "GetUsuariosBySucursal", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular"] },
                { "ObtenerUsuarios", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular"] },
                { "EliminarUsuario", ["Super Admin"] },
                { "CambiarPassword", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Empleado Farmacia", "Medico", "Paciente", "Particular"] },
                { "CrearActualizarImagenFirma", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Empleado Farmacia", "Medico", "Paciente", "Particular"] },
                { "ObtenerFirma", ["Super Admin", "Supervisor Sucursales", "Medico", "Particular"] },
                { "EliminarFirma", ["Super Admin"] },
                { "ObtenerImagen", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Empleado Farmacia", "Medico", "Paciente", "Particular"] },
                { "EliminarImagen", ["Super Admin"] }
            };
        }

        public static class PacientesController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosPacientesController =
            [
                "Super Admin",
                "Medico",
                "Particular",
                "Paciente"
            ];

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesPacientesController = new()
            {
                { "GetPacienteByIdUsuario", ["Super Admin", "Medico", "Paciente", "Particular"] },
                { "GetPacienteByIdPaciente", ["Super Admin", "Medico", "Particular"] },
                { "GetPacienteByName", ["Super Admin", "Medico", "Particular"] },
                { "GetPacientesByEntidadNacimiento", ["Super Admin", "Medico"] },
                { "GetPacientesBySucursal", ["Super Admin", "Medico", "Particular"] },
                { "GetPacientesByGEMP", ["Super Admin", "Medico", "Particular"] },
                { "GetPacientesByMedico", ["Super Admin", "Medico" , "Particular"] },
                { "CreatePaciente", ["Super Admin", "Medico", "Particular"] },
                { "UpdatePaciente", ["Super Admin", "Medico", "Particular"] },
                { "GetEntidadesFederativas", ["Super Admin", "Medico", "Particular"] },
                { "EliminarPaciente", ["Super Admin"] },
                { "GetEventosSaludByPaciente", ["Paciente"] },
                { "GetEventoSaludById", ["Paciente"] },
                { "CreateEventoSalud", ["Paciente"] },
                { "UpdateEventoSalud", ["Paciente"] },
                { "DeleteEventoSalud", ["Paciente"] },
                { "GenerarQRParaPaciente", ["Paciente"] }

            };
        }



        public static class ConsultaController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosConsultaController =
            [
                "Super Admin",
                "Medico",
                "Particular",
                "Supervisor Sucursales",
                "Responsable Farmacia",
                "Paciente"
            ];
            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesConsultaController = new()
            {
                { "GetIdsFromLink", ["Super Admin", "Medico", "Particular", "Supervisor Sucursales","Paciente"] },
                { "AnalyzePrescription", ["Super Admin", "Medico", "Particular"] },
                { "AnalyzePrescriptionXML", ["Super Admin", "Medico", "Particular"] },
                { "SearchPacienteByName", ["Super Admin", "Medico", "Particular"] },
                { "GetMedicamentoByName", ["Super Admin", "Medico", "Particular", "Responsable Farmacia"] },
                { "ObtenerSucursalesPorUsuario", ["Super Admin", "Medico", "Responsable Farmacia"] },
                { "GetTieneEventoSalud", ["Super Admin", "Medico" , "Particular", "Supervisor Sucursales", "Responsable Farmacia"] },
                { "GetTieneEventoMedicamentoso", ["Super Admin", "Medico" , "Particular", "Supervisor Sucursales", "Responsable Farmacia"] },
                { "GetEventosSaludByPaciente", ["Super Admin", "Medico" , "Particular", "Supervisor Sucursales", "Responsable Farmacia"] }
            };
        }

        public static class RecetaController
        {
            public static readonly string[] RolesPermitidosRecetaController =
            [
                "Super Admin",
                "Medico",
                "Particular",
                "Supervisor Sucursales",
                "Responsable Farmacia",
                "Empleado Farmacia",
                "Paciente"
            ];
            public static readonly Dictionary<string, string[]> EndpointRolesRecetaController = new()
            {
                { "GetFilteredRecetas", ["Super Admin", "Medico", "Particular", "Supervisor Sucursales", "Responsable Farmacia"] },
                { "GetFilteredRecetasByIdMedico", ["Medico", "Particular"] },
                { "GetRecetaByIdReceta", ["Super Admin", "Medico", "Particular", "Supervisor Sucursales", "Responsable Farmacia","Paciente"] },
                { "GetRecetasByIdPaciente", ["Super Admin", "Medico", "Particular", "Supervisor Sucursales", "Responsable Farmacia", "Paciente"] },
                { "GetQRByIdReceta", ["Paciente"] },
                { "RegistrarReceta", ["Super Admin", "Medico", "Particular"] },
                { "ActualizarReceta", ["Super Admin", "Medico", "Particular"] },
                { "TimbrarReceta", ["Medico", "Particular"] },
                { "EliminarReceta", ["Super Admin", "Medico", "Particular"] },
                { "ConsultarReceta", ["Super Admin", "Medico", "Particular"] },
                { "ObtenerRecetasPorMedico", ["Super Admin", "Medico", "Particular"] },
                { "ObtenerRecetasByIdMedico", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia"] },
                { "GetRecetaUpdateByIdReceta", ["Super Admin", "Medico", "Particular"] }

            };
        }
        public static class DetalleRecetaController
        {
            public static readonly string[] RolesPermitidosDetalleRecetaController =
            [
                "Super Admin",
                "Medico",
                "Particular",
                "Responsable Farmacia",
                "Empleado Farmacia"
            ];

            public static readonly Dictionary<string, string[]> EndpointRolesDetalleRecetaController = new()
            {
                { "GetDetallesByIdReceta", ["Super Admin", "Medico", "Particular"] },
                { "CreateUpdateReaccion", ["Super Admin", "Medico", "Particular"] },
                { "DeleteReaccion", ["Super Admin", "Medico", "Particular"] },
                { "GetSoloMedicamentosActivos", ["Super Admin", "Medico", "Particular", "Responsable Farmacia", "Empleado Farmacia"] },
                { "GetReaccionMedicamentoPrevio", ["Super Admin", "Medico", "Particular"] }
            };
        }

        public static class PuntoVentaController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosPuntoVentaController =
            [
                "Super Admin",
                "Supervisor Sucursales",
                "Responsable Farmacia",
                "Empleado Farmacia"
            ];

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesPuntoVentaController = new()
            {

                { "ConsultarRecetaPorId", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Empleado Farmacia"] },
                { "SurtirMedicamentos", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Empleado Farmacia"] },
                { "BuscarReceta", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Empleado Farmacia"] }
            };
        }


        public static class DashBoardController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosDashBoardController =
            [
                "Super Admin",
                "Supervisor Sucursales",
                "Responsable Farmacia",
                "Medico",
                "Particular"
            ];

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesDashBoardController = new()
            {

                { "GetKpiPacientesRecetas", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular"] },
                { "GetSucursalPacientes", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular"] }
            };
        }
        public static class VidalCim10Controller
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosVidalCim10Controller =
            [
                "Super Admin",
                "Supervisor Sucursales",
                "Responsable Farmacia",
                "Medico",
                "Particular"
            ];

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesVidalCim10Controller = new()
            {

                { "GetCIM10ByName", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular"] },
                { "GetPSumistroByIdProduct", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular"] },
                { "AskPSumistroByIdProduct", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular"] }
            };
        }
        public static class VIDAL_AllergyController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosVIDAL_AllergyController =
            [
                "Super Admin",
                "Supervisor Sucursales",
                "Responsable Farmacia",
                "Medico",
                "Particular"
            ];

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesVIDAL_AllergyController = new()
            {
                { "GetAllergiesByName", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular"] },
            };
        }
        public static class VIDAL_MoleculeController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosVIDAL_MoleculeController =
            [
                "Super Admin",
                "Supervisor Sucursales",
                "Responsable Farmacia",
                "Medico",
                "Particular"
            ];

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesVIDAL_MoleculeController = new()
            {
                { "GetMoleculeByName", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular"] },
            };
        }


        public static class CatalogoController
        {
            public static readonly string[] RolesPermitidosCatalogoController =
            [
                "Super Admin",
            "Supervisor Sucursales",
            "Responsable Farmacia",
            "Medico",
            "Particular",
            "Paciente",
            "Empleado Farmacia"
            ];

            public static readonly Dictionary<string, string[]> EndpointRolesCatalogoController = new()
            {

                { "SearchAsentamiento", ["Super Admin", "Medico", "Particular", "Responsable Farmacia"] },
                // Entidades
                { "GetAllEntidades", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular", "Paciente", "Empleado Farmacia"] },
                { "GetEntidadById", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular", "Paciente", "Empleado Farmacia"] },
                { "CreateEntidad", ["Super Admin"] },
                { "UpdateEntidad", ["Super Admin"] },
                { "DeleteEntidad", ["Super Admin"] },

                // Municipios
                { "GetAllMunicipiosByIdEntidad", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular", "Paciente", "Empleado Farmacia"] },
                { "GetMunicipioById", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular", "Paciente" , "Empleado Farmacia"] },
                { "CreateMunicipio", ["Super Admin"] },
                { "UpdateMunicipio", ["Super Admin"] },
                { "DeleteMunicipio", ["Super Admin"] },

                // Tipo Asentamiento
                { "GetAllTipoAsentamientos", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular", "Paciente" , "Empleado Farmacia"] },
                { "GetTipoAsentamientoById", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular", "Paciente" , "Empleado Farmacia"] },
                { "GetTipoAsentamientosByName", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular", "Paciente" , "Empleado Farmacia"] },
                { "CreateTipoAsentamiento", ["Super Admin"] },
                { "UpdateTipoAsentamiento", ["Super Admin"] },
                { "DeleteTipoAsentamiento", ["Super Admin"] },

                // CatCP
                { "GetAllCP", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular", "Paciente" , "Empleado Farmacia"] },
                { "GetAllCPByMunicipio", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular", "Paciente" , "Empleado Farmacia"] },
                { "GetAllCPByEntidad", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular", "Paciente" , "Empleado Farmacia"] },
                { "GetCPById", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular", "Paciente" , "Empleado Farmacia"] },
                { "CreateCP", ["Super Admin"] },
                { "UpdateCP", ["Super Admin"] },
                { "DeleteCP", ["Super Admin"] },

                // Ciudades
                { "GetAllCiudades", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular", "Paciente" , "Empleado Farmacia"] },
                { "GetCiudadById", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular", "Paciente" , "Empleado Farmacia"] },
                { "GetCiudadesByName", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular", "Paciente" , "Empleado Farmacia"] },
                { "CreateCiudad", ["Super Admin"] },
                { "UpdateCiudad", ["Super Admin"] },
                { "DeleteCiudad", ["Super Admin"] },

                // Eventos de Salud
                { "GetAllEventosSalud", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular", "Paciente" , "Empleado Farmacia"] },
                { "GetEventoSaludById", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular", "Paciente" , "Empleado Farmacia"] },
                { "CreateEventoSalud", ["Super Admin"] },
                { "UpdateEventoSalud", ["Super Admin"] },
                { "DeleteEventoSalud", ["Super Admin"] },

                // Asentamientos
                { "GetAllAsentamientos", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular", "Paciente" , "Empleado Farmacia"] },
                { "GetAsentamientoById", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular", "Paciente" , "Empleado Farmacia"] },
                { "CreateAsentamiento", ["Super Admin"] },
                { "UpdateAsentamiento", ["Super Admin"] },
                { "DeleteAsentamiento", ["Super Admin"] },

                // Asentamiento Ciudad
                { "GetAllAsentamientoCiudad", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular", "Paciente" , "Empleado Farmacia"] },
                { "GetAsentamientoCiudadById", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular", "Paciente" , "Empleado Farmacia"] },
                { "CreateAsentamientoCiudad", ["Super Admin"] },
                { "UpdateAsentamientoCiudad", ["Super Admin"] },
                { "DeleteAsentamientoCiudad", ["Super Admin"] },

                // Domicilio completo
                { "InsertarDomicilioCompleto", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular"] }
            };
        }

        public static class VIDAL_ToolsController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosVIDAL_ToolsController =
            [
                "Super Admin",
                "Supervisor Sucursales",
                "Responsable Farmacia",
                "Medico",
                "Particular"
            ];

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesVIDAL_ToolsController = new()
            {

                { "GetFichaHtml", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular"] },
                { "GetTiposDeAlertas", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular"] },
                { "GetAlertaByTipo", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular"] },
                { "GetNoticiaById", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular"] },
                { "GetNoticiaRelatedById", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular"] },
                { "GetDocumentosFicha", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular"] }
            };
        }
     
        public static class AlertasController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosAlertasController =
            [
                "Super Admin",
                "Paciente",
            ];
            
            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesAlertasController = new()
            {
                { "ActivarAlertaToma", ["Super Admin", "Paciente"] },
                { "DesactivarAlertaToma", ["Super Admin", "Paciente"] }
            };
        }
        public static class AlertaManualController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosAlertaManualController =
            [
                "Super Admin",
                "Paciente",
            ];

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesAlertaManualController = new()
            {
                { "BuscarPorNombre", ["Super Admin", "Paciente"] },
                { "ActivarAlertaTomaManual", ["Super Admin", "Paciente"] },
                { "DesactivarAlertaTomaManual", ["Super Admin", "Paciente"] }
            };
        }
        
        public static class RecetaCatalogosController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosRecetaCatalogosController =
            [
                "Super Admin",
                "Supervisor Sucursales",
                "Responsable Farmacia",
                "Medico",
                "Particular",
                "Paciente",
            ];

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesRecetaCatalogosController = new()
            {
                { "ObtenerFrecuencyTypes", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular", "Paciente"] },
                { "ObtenerFrecuencyTypePorId", ["Super Admin", "Supervisor Sucursales", "Responsable Farmacia", "Medico", "Particular", "Paciente"] }
            };
        }

        public static class TutoresController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosTutoresController =
            [
                "Super Admin",
                "Supervisor ucursales",
                "Particular",
                "Medico",
                "Paciente",
                "Responsable Farmacia",
                "Empleado Farmacia"
            ];

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesTutoresController = new()
            {
                { "AsignarTutor", ["Super Admin", "Supervisor Sucursales", "Particular", "Medico"] },
                { "ActualizarTutor", ["Super Admin", "Supervisor Sucursales", "Particular", "Medico"] },
                { "EliminarTutor", ["Super Admin"] },
                { "GetTutorPorPaciente", ["Super Admin", "Particular", "Medico", "Supervisor Sucursales", "Paciente"] }
            };
        }

        public static class AlertasProgramadasController
        {
            // Roles con acceso al controlador completo
            public static readonly string[] RolesPermitidosAlertasProgramadasController =
            [
                "Super Admin",
                "Supervisor ucursales",
                "Particular",
                "Medico",
                "Paciente",
                "Responsable Farmacia",
                "Empleado Farmacia"
            ];

            // Permisos por endpoint
            public static readonly Dictionary<string, string[]> EndpointRolesAlertasProgramadasController = new()
            {
                { "GetAlertasProgramadas", ["Super Admin", "Paciente"] },
                { "GetAlertasProgramadasEnFecha", ["Super Admin", "Paciente"] }
                
            };
        }
    }
}
