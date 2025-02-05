using RMD.Data;
using RMD.Extensions;
using RMD.Interface.Recetas;
using RMD.Models.Recetas;


namespace RMD.Service.Recetas
{
    public class RecetaService : IRecetaService
    {
        private readonly RecetasDbContext _context;

        public RecetaService(RecetasDbContext context)
        {
            _context = context;
        }

        public async Task<RecetaRecibidaModel> GetRecetaByIdRecetaByMedicoAsync(Guid idReceta, Guid idUsuario)
        {
            var idRecetaParam = new SqlParameter("@IdReceta", idReceta);
            var idUsuarioParam = new SqlParameter("@IdUsuario", idUsuario);
            var outputMessageParam = new SqlParameter("@OutputMessage", SqlDbType.NVarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            // Ejecutar el SP para obtener la receta y sus detalles
            //var receta = await _context.RecetaWithDetalles
            //    .FromSqlRaw("EXEC Receta_GetByIdRecetaByMedico @IdReceta, @IdUsuario, @OutputMessage OUTPUT",
            //                idRecetaParam, idUsuarioParam, outputMessageParam)
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync();
            var receta = _context.RecetaWithDetalles
            .FromSqlRaw("EXEC Receta_GetByIdRecetaByMedico @IdReceta, @IdUsuario, @OutputMessage OUTPUT",
                        idRecetaParam, idUsuarioParam, outputMessageParam)
            .AsEnumerable()  // Procesa la composición en el cliente
            .FirstOrDefault();
            var outputMessage = outputMessageParam.Value.ToString();
            Console.WriteLine($"Resultado del SP: {outputMessage}");

            if (receta == null || outputMessage.Contains("Error"))
            {
                return null;
            }

            // Obtener las listas de alergias, molecules y patologías
            var alergias = await ObtenerAlergiasPorIdsAsync(receta.Alergias);  // Debería devolver List<AllergyModel>
            var molecules = await ObtenerMoleculesPorIdsAsync(receta.Molecules);  // Debería devolver List<MoleculeModel>
            var patologias = await ObtenerCIM10PorIdsAsync(receta.Patologias);  // Debería devolver List<CIM10Model>

            // Crear el modelo final RecetaRecibidaModel con los detalles y las listas de alergias, molecules, y patologías
            var recetaRecibida = new RecetaRecibidaModel
            {
                IdReceta = receta.IdReceta == Guid.Empty ? Guid.NewGuid() : receta.IdReceta,  // Si es Guid.Empty, generamos un nuevo GUID
                IdMedico = receta.IdMedico,
                IdPaciente = receta.IdPaciente,
                PacPeso = receta.PacPeso,  // Sin ?? si es un decimal no nullable
                PacTalla = receta.PacTalla,  // Sin ?? si es un decimal no nullable
                PacEmbarazo = receta.PacEmbarazo,
                PacSemAmenorrea = receta.PacSemAmenorrea ?? 0,  // Si es nullable, usamos 0 como valor por defecto
                PacLactancia = receta.PacLactancia,
                PacCreatinina = receta.PacCreatinina ?? 0m,  // Si es nullable, usamos 0m como valor por defecto
                Alergias = alergias,  // Asigna la lista correcta de AllergyModel
                Molecules = molecules,  // Asigna la lista correcta de MoleculeModel
                Patologias = patologias,  // Lista de CIM10Model obtenida del SP
                IdSucursal = receta.IdSucursal,
                IdGEMP = receta.IdGEMP,
                DetallesReceta = receta.DetallesReceta.Select(detalle => new RecetaDetalleRecibidaModel
                {
                    IdDetalleReceta = detalle.IdDetalleReceta ?? Guid.NewGuid(),  // Si es nullable, generamos un nuevo GUID
                    MedicamentoId = detalle.MedicamentoId,
                    MedicamentoType = detalle.MedicamentoType,
                    CantidadDiaria = detalle.CantidadDiaria,  // Si es nullable, asignar 0m
                    UnidadDispensacionId = detalle.UnidadDispensacionId,
                    RutaAdministracionId = detalle.RutaAdministracionId,
                    Indicacion = detalle.Indicacion,
                    IndicacionNombre = detalle.IndicacionNombre,  // Nuevo campo
                    Frecuencia = detalle.Frecuencia ?? string.Empty,  // Nuevo campo, asignar cadena vacía si es null
                    Observaciones = detalle.Observaciones,  // Nuevo campo
                    Duracion = detalle.Duracion,  // Si es nullable, asignar 0
                    UnidadDuracion = detalle.UnidadDuracion,
                    PeriodoInicio = detalle.PeriodoInicio ?? DateTime.Now,  // Si es nullable, asignar fecha actual
                    PeriodoTerminacion = detalle.PeriodoTerminacion ?? DateTime.Now  // Si es nullable, asignar fecha actual
                }).ToList()  // Detalles de receta obtenidos directamente del SP
            };

            return recetaRecibida;
        }

        public async Task<Receta> GetRecetaByIdAsync(Guid idReceta)
        {
            var idParam = new SqlParameter("@IdReceta", idReceta);

            var results = await _context.Receta
                .FromSqlRaw("EXEC GetRecetaById @IdReceta", idParam)
                .ToListAsync();

            if (results.Count == 0)
            {
                throw new KeyNotFoundException($"No se encontró una receta con el Id {idReceta}");
            }

            return results[0];
        }

        public async Task<IEnumerable<Receta>> GetRecetasByMedicoAsync(Guid idMedico)
        {
            var idParam = new SqlParameter("@IdMedico", idMedico);

            return await _context.Receta
                .FromSqlRaw("EXEC GetRecetasByMedico @IdMedico", idParam)
                .ToListAsync();
        }

        public async Task<IEnumerable<Receta>> GetRecetasByPacienteAsync(Guid idPaciente)
        {
            var idParam = new SqlParameter("@IdPaciente", idPaciente);

            return await _context.Receta
                .FromSqlRaw("EXEC GetRecetasByPaciente @IdPaciente", idParam)
                .ToListAsync();
        }

        public async Task<IEnumerable<RecetaList>> GetFilteredRecetasAsync(string roleId, Guid idGEMP, Guid? idSucursal, DateTime? startDate, DateTime? endDate, string dateFilter)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Receta_GetFilteredRecetas", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Agregar parámetros necesarios
                command.Parameters.AddWithValue("@RoleId", roleId);
                command.Parameters.AddWithValue("@IdGEMP", idGEMP);

                if (idSucursal.HasValue)
                    command.Parameters.AddWithValue("@IdSucursal", idSucursal.Value);
                else
                    command.Parameters.AddWithValue("@IdSucursal", DBNull.Value);

                if (startDate.HasValue)
                    command.Parameters.AddWithValue("@StartDate", startDate.Value);
                else
                    command.Parameters.AddWithValue("@StartDate", DBNull.Value);

                if (endDate.HasValue)
                    command.Parameters.AddWithValue("@EndDate", endDate.Value);
                else
                    command.Parameters.AddWithValue("@EndDate", DBNull.Value);

                command.Parameters.AddWithValue("@DateFilter", dateFilter ?? (object)DBNull.Value);

                using var reader = await command.ExecuteReaderAsync();
                var recetas = new List<RecetaList>();

                while (await reader.ReadAsync())
                {
                    recetas.Add(new RecetaList
                    {
                        IdReceta = reader.GetGuid(reader.GetOrdinal("IdReceta")),
                        IdMedico = reader.GetGuid(reader.GetOrdinal("IdMedico")),
                        IdPaciente = reader.GetGuid(reader.GetOrdinal("IdPaciente")),
                        NombreMedico = reader.GetString(reader.GetOrdinal("NombreMedico")),
                        NombrePaciente = reader.GetString(reader.GetOrdinal("NombrePaciente")),
                        IdSucursal = reader.GetGuid(reader.GetOrdinal("IdSucursal")),
                        IdGEMP = reader.GetGuid(reader.GetOrdinal("IdGEMP")),
                        FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                        FechaUltimaModificacion = reader.GetDateTime(reader.GetOrdinal("FechaUltimaModificacion"))
                    });
                }

                return recetas;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener recetas filtradas: {ex.Message}");
            }
        }

        public async Task<string> GetRecetaByIdRecetaAsync(Guid idReceta, Guid idPaciente)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Recetas_GetRecetaByIdReceta", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdReceta", idReceta);
                command.Parameters.AddWithValue("@IdPaciente", idPaciente);

                Receta_PacienteRequest receta = null;
                var detalles = new List<Receta_RecetaDetalleRequest>();
                Receta_FormatoRequest formato = null;

                using (var reader = await command.ExecuteReaderAsync())
                {
                    // Leer primera tabla
                    if (await reader.ReadAsync())
                    {
                        receta = new Receta_PacienteRequest
                        {
                            IdReceta = reader.GetGuid(reader.GetOrdinal("IdReceta")),
                            IdMedico = reader.GetGuid(reader.GetOrdinal("IdMedico")),
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
                            IdPaciente = reader.GetGuid(reader.GetOrdinal("IdPaciente")),
                            NombresPaciente = reader["NombresPacientes"]?.ToString(),
                            PrimerApellidoPaciente = reader["PrimerApellidoPacientes"]?.ToString(),
                            SegundoApellidoPaciente = reader["SegundoApellidoPacientes"]?.ToString(),
                            EdadPaciente = reader.GetInt32(reader.GetOrdinal("EdadPaciente")),
                            PacPeso = reader.GetDecimal(reader.GetOrdinal("PacPeso")),
                            Genero = reader["Genero"]?.ToString(),
                            PacTalla = reader.GetDecimal(reader.GetOrdinal("PacTalla")),
                            Diagnosticos = reader["Diagnosticos"]?.ToString(),
                            IdGEMP = reader.GetGuid(reader.GetOrdinal("IdGEMP")),
                            NombreGrupoEmpresarial = reader["NombreGrupoEmpresarial"]?.ToString(),
                            LogoBase64 = reader["LogoBase64"]?.ToString(),
                            IdSucursal = reader.GetGuid(reader.GetOrdinal("IdSucursal")),
                            NumeroSucursal = reader["NumeroSucursal"]?.ToString(),
                            NombreSucursal = reader["NombreSucursal"]?.ToString(),
                            RegistroSanitario = reader["RegistroSanitario"]?.ToString(),
                            Domicilio = reader["Domicilio"]?.ToString(),
                            IdAsentamiento = reader.GetInt32(reader.GetOrdinal("IdAsentamiento")),
                            NombreAsentamiento = reader["NombreAsentamiento"]?.ToString(),
                            IdTipoAsentamiento = reader.GetInt32(reader.GetOrdinal("IdTipoAsentamiento")),
                            TipoAsentamiento = reader["TipoAsentamiento"]?.ToString(),
                            IdCP = reader.GetInt32(reader.GetOrdinal("IdCP")),
                            CodigoPostal = reader["CodigoPostal"]?.ToString(),
                            IdMunicipio = reader.GetInt32(reader.GetOrdinal("IdMunicipio")),
                            NombreMunicipio = reader["NombreMunicipio"]?.ToString(),
                            IdCiudad = reader.GetInt32(reader.GetOrdinal("IdCiudad")),
                            NombreCiudad = reader["NombreCiudad"]?.ToString(),
                            IdEntidad = reader.GetInt32(reader.GetOrdinal("IdEntidad")),
                            Estado = reader["Estado"]?.ToString(),
                            Abreviatura = reader["Abreviatura"]?.ToString(),
                            Fecha = DateTime.TryParse(reader["Fecha"]?.ToString(), out DateTime fecha) ? fecha : DateTime.Now
                        };
                    }

                    // Leer segunda tabla
                    if (await reader.NextResultAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            detalles.Add(new Receta_RecetaDetalleRequest
                            {
                                IdDetalleReceta = reader.GetGuid(reader.GetOrdinal("IdDetalleReceta")),
                                IdReceta = reader.GetGuid(reader.GetOrdinal("IdReceta")),
                                MedicamentoType = reader["MedicamentoType"]?.ToString(),
                                MedicamentoId = reader.GetInt32(reader.GetOrdinal("MedicamentoId")),
                                MedicamentoNombre = reader["MedicamentoNombre"]?.ToString(),
                                UnidadDispensacionId = reader.GetInt32(reader.GetOrdinal("UnidadDispensacionId")),
                                UnidadDispensacion = reader["UnidadDispensacion"]?.ToString(),
                                RutaAdministracionId = reader.GetInt32(reader.GetOrdinal("RutaAdministracionId")),
                                RutaAdministracion = reader["RutaAdministracion"]?.ToString(),
                                CantidadDiaria = reader.GetDecimal(reader.GetOrdinal("CantidadDiaria")),
                                Indicacion = reader["Indicacion"]?.ToString(),
                                IndicacionNombre = reader["IndicacionNombre"]?.ToString(),
                                Frecuencia = reader["Frecuencia"]?.ToString(),
                                Observaciones = reader["Observaciones"]?.ToString(),
                                Duracion = reader.GetInt32(reader.GetOrdinal("Duracion")),
                                UnidadDuracion = reader["UnidadDuracion"]?.ToString(),
                                PeriodoInicio = reader.GetDateTime(reader.GetOrdinal("PeriodoInicio")),
                                PeriodoTerminacion = reader.IsDBNull(reader.GetOrdinal("PeriodoTerminacion"))
                                    ? (DateTime?)null
                                    : reader.GetDateTime(reader.GetOrdinal("PeriodoTerminacion")),
                                Surtido = reader.GetBoolean(reader.GetOrdinal("Surtido"))
                            });
                        }
                    }

                    // Leer tercera tabla
                    if (await reader.NextResultAsync() && await reader.ReadAsync())
                    {
                        formato = new Receta_FormatoRequest
                        {
                            IdFormatoReceta = reader.GetInt32(reader.GetOrdinal("IdFormatoReceta")),
                            Formato = reader["Formato"]?.ToString(),
                            Logo = reader["Logo"]?.ToString(),
                            LogoSuperior = reader["LogoSuperior"]?.ToString(),
                            LogoInferior = reader["LogoInferior"]?.ToString()
                        };
                    }
                }

                // Generar HTML con el helper
                if (receta != null && formato != null)
                {
                    var htmlContent = RecetaPdfExtension.GenerarHtmlReceta(receta, detalles, formato);
                    byte[] pdfBytes = [0, 1, 2];
                    //byte[] pdfBytes = ConvertHtmlToPdf(htmlContent);
                    /*return ConvertHtmlToPdf(htmlContent);*/ // Retorna el PDF como byte[]
                    return htmlContent;                    
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la receta: {ex.Message}");
            }
        }


        public async Task<List<RecetaList>> GetRecetasByIdPacienteAsync(Guid idPaciente, DateTime? startDate, DateTime? endDate, string dateFilter)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("Recetas_GetRecetasByIdPaciente", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdPaciente", idPaciente);

                var recetas = new List<RecetaList>();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        recetas.Add(new RecetaList
                        {
                            IdReceta = reader.GetGuid(reader.GetOrdinal("IdReceta")),
                            IdMedico = reader.GetGuid(reader.GetOrdinal("IdMedico")),
                            IdPaciente = reader.GetGuid(reader.GetOrdinal("IdPaciente")),
                            NombreMedico = reader.GetString(reader.GetOrdinal("NombreMedico")),
                            NombrePaciente = reader.GetString(reader.GetOrdinal("NombrePaciente")),
                            IdSucursal = reader.GetGuid(reader.GetOrdinal("IdSucursal")),
                            IdGEMP = reader.GetGuid(reader.GetOrdinal("IdGEMP")),
                            FechaCreacion = reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                            FechaUltimaModificacion = reader.GetDateTime(reader.GetOrdinal("FechaUltimaModificacion"))
                        });
                    }
                }

                return recetas;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener las recetas del paciente: {ex.Message}");
            }
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

        private string ObtenerValorDesdeToken(string token, string claimType)
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == claimType);

            return claim?.Value ?? string.Empty;
        }

        private string ConvertirListaAString(List<int> lista)
        {
            return string.Join(",", lista);
        }

        private List<RequestSearchAllergy> ParseAllergies(string allergiesString)
        {
            return allergiesString.Split(',')
                .Select(a =>
                {
                    var parts = a.Split('-');
                    return new RequestSearchAllergy
                    {
                        IdAllergy = int.Parse(parts[0].Trim()),
                        NameAllergy = parts[1].Trim()
                    };
                }).ToList();
        }

        private List<RequestSearchMolecules> ParseMolecules(string moleculesString)
        {
            return moleculesString.Split(',')
                .Select(m =>
                {
                    var parts = m.Split('-');
                    return new RequestSearchMolecules
                    {
                        IdMolecule = int.Parse(parts[0].Trim()),
                        NameMolecule = parts[1].Trim()
                    };
                }).ToList();
        }

        private List<RequestSearchCIM10> ParseCIM10(string cim10String)
        {
            return cim10String.Split(',')
                .Select(c =>
                {
                    var parts = c.Split('-');
                    return new RequestSearchCIM10
                    {
                        IdCIM10 = int.Parse(parts[0].Trim()),
                        NameCIM10 = parts[1].Trim(),
                        Code = parts[2].Trim() // Ahora se guarda como string
                    };
                }).ToList();
        }

        //private byte[] ConvertHtmlToPdf(string htmlContent)
        //{
        //    var converter = new SynchronizedConverter(new PdfTools());

        //    var doc = new HtmlToPdfDocument()
        //    {
        //        GlobalSettings = new GlobalSettings
        //        {
        //            ColorMode = ColorMode.Color,
        //            Orientation = Orientation.Portrait,
        //            PaperSize = PaperKind.A4,
        //            Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 }
        //        }
        //    };

        //    // Agregar el objeto a la lista de objetos del documento
        //    doc.Objects.Add(new ObjectSettings
        //    {
        //        HtmlContent = htmlContent,
        //        WebSettings = new WebSettings { DefaultEncoding = "utf-8" }
        //    });

        //    return converter.Convert(doc);
        //}




        








    }
}
