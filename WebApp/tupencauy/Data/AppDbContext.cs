using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using tupencauy.Models;

namespace tupencauy.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<AppUser> Usuarios { get; set; }
        public DbSet<Sitio> Sitios { get; set; }
        public DbSet<Penca> Pencas { get; set; }
        public DbSet<EventoDeportivo> EventosDeportivos { get; set; }
        public DbSet<UnoVsUno> UnoVsUno { get; set; }
        public DbSet<FreeForAll> FreeForAll { get; set; }
        public DbSet<PencaSitio> PencasSitio { get; set; }
        public DbSet<PencaSitioUsuario> PencaSitioUsuario { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Prediccion> Prediccion { get; set; }
        public DbSet<Sistema> Sistema { get; set; }
        public DbSet<Recarga> Recargas { get; set; }

        protected override async void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configuración de TPH para EventoDeportivo y sus subclases
            builder.Entity<EventoDeportivo>()
                   .HasDiscriminator<string>("TipoDeEvento")
                   .HasValue<UnoVsUno>("UnoVsUno")
                   .HasValue<FreeForAll>("FreeForAll");

            // Configuración de las relaciones entre Penca y EventoDeportivo
            builder.Entity<Penca>()
                   .HasMany(p => p.EventosDeportivos)
                   .WithMany(e => e.Pencas)
                   .UsingEntity<Dictionary<string, object>>(
                       "PencaEvento",
                       j => j.HasOne<EventoDeportivo>().WithMany().HasForeignKey("IdEventoDeportivo"),
                       j => j.HasOne<Penca>().WithMany().HasForeignKey("IdPenca"),
                       j =>
                       {
                           j.HasKey("IdPenca", "IdEventoDeportivo");
                           j.ToTable("PencaEventos");
                       });

            // Seed Sites
            var sitiosApuestas = new[]
            {
            "Bet365", "William Hill", "Bwin", "Betfair", "Unibet",
            "888sport", "Ladbrokes", "Paddy Power", "Betway", "Pinnacle"
            };

            var tiposRegistro = new[] { "Abierto", "Cerrado", "Con aprobacion", "Invitacion Link" };

            var sites = new List<Sitio>();
            for (int i = 0; i < 10; i++)
            {
                sites.Add(new Sitio
                {
                    Id = i + 1,
                    Nombre = sitiosApuestas[i],
                    Url = $"www.{sitiosApuestas[i].ToLower().Replace(" ", "")}.com",
                    TipoRegistro = tiposRegistro[i % tiposRegistro.Length],
                    ColorPrincipal = "#000000",
                    ColorSecundario = "#000000",
                    ColorTipografia = "#000000",
                    Status = i % 2 == 0,
                    cantidadUsuarios = 100,
                    TenantId = Guid.NewGuid().ToString()
                });
            }

            builder.Entity<Sitio>().HasData(sites);

            // Seed Roles (remains the same)
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "49aa311f-53c3-40a6-9ad4-11c7cb64fe4d", Name = "SuperAdmin", NormalizedName = "SUPERADMIN" },
                new IdentityRole { Id = "839311ef-70e2-4198-8607-0cd637dd385a", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "adece3c4-4e32-4783-b5a7-fba4da010e9b", Name = "Usuario", NormalizedName = "USUARIO" }
            );

            // Seed Users
            var hasher = new PasswordHasher<AppUser>();
            var users = new List<AppUser>();
            var userRoles = new List<IdentityUserRole<string>>();

            string[] nombres = {
                "Juan Pérez", "María González", "Pedro Rodríguez", "Ana Martínez", "Luis Sánchez",
                "Carmen López", "Miguel Fernández", "Isabel García", "José Díaz", "Laura Moreno",
                "Carlos Ruiz", "Sofía Torres", "Antonio Jiménez", "Elena Romero", "Francisco Navarro",
                "Marta Alonso", "David Gutiérrez", "Lucía Muñoz", "Javier Domínguez", "Beatriz Álvarez",
                "Manuel Vázquez", "Rosa Ramos", "Alejandro Serrano", "Cristina Ortega", "Andrés Castro",
                "Paula Rubio", "Raúl Molina", "Silvia Ortiz", "Fernando Morales", "Nuria Delgado",
                "Alberto Méndez", "Raquel Herrera", "Roberto Marín", "Marina Guerrero", "Ángel Cabrera",
                "Eva Prieto", "Sergio Vega", "Natalia Cruz", "Jorge Flores", "Celia Campos",
                "Daniel Calvo", "Alicia Ibáñez", "Pablo Caballero", "Sara León", "Rubén Román",
                "Andrea Santos", "Adrián Castillo", "Teresa Garrido", "Álvaro Lozano", "Mónica Herrero",
                "Diego Velasco", "Inés Peña", "Ismael Santana", "Esther Crespo", "Hugo Arias",
                "Noelia Vargas", "Iván Giménez", "Lorena Carmona", "Víctor Sanz", "Patricia Benítez",
                "Eduardo Ferrer", "Aurora Mora", "Guillermo Gallego", "Irene Aguilar", "Enrique Méndez",
                "Rocío Iglesias", "Tomás Durán", "Yolanda Pascual", "Samuel Rojas", "Verónica Bravo",
                "Mario Hidalgo", "Elisa Moya", "Marcos Santos", "Lidia Soler", "Arturo Parra",
                "Miriam Gallardo", "Ignacio Esteban", "Nerea Nieto", "Gonzalo Fuentes", "Julia Medina",
                "Ricardo Santamaría", "Diana Vicente", "Salvador Pascual", "Sonia Ballesteros", "Lorenzo Reyes",
                "Carla Ferrer", "Jesús Aguilera", "Tania Benítez", "Nicolás Hidalgo", "Susana Santana",
                "Joaquín Herrera", "Claudia Duarte", "Felipe Vargas", "Ainara Sáez", "Rodrigo Pardo",
                "Alba Merino", "Óscar Moya", "Vanesa Crespo", "Jaime Giménez", "Ainhoa Soler",
                "Ramón Gallardo", "Lara Blanco", "Ernesto Bravo", "Noa Vidal", "Emilio Esteban",
                "Carolina Ibáñez", "Julio Caballero", "Sandra León", "Borja Santos", "Mireia Arias",
                "Alfredo Calvo", "Aitana Ortega", "Xavier Rivas", "Ariadna Castro", "Ismael Aguilar",
                "Carlota Romero", "Esteban Marín", "Judith Sáez", "Darío Rubio", "Mar Ortiz",
                "Rafael Morales", "Irene Delgado", "Joel Méndez", "Candela Guerrero", "Aitor Cabrera",
                "Abril Prieto", "Marc Vega", "Lola Cruz", "Bruno Flores", "Alma Campos",
                "Héctor Peña", "Vera Santana", "Unai Crespo", "Adriana Arias", "Martín Vargas",
                "Daniela Gallego", "Iker Aguilar", "Valeria Méndez", "Pau Iglesias", "Vega Durán",
                "Leo Pascual", "Olivia Rojas", "Asier Bravo", "Jimena Hidalgo", "Mateo Moya",
                "Alexia Soler", "Fabio Parra", "Leire Gallardo", "Nil Esteban", "Zoe Nieto",
                "Alonso Fuentes", "Chloe Medina", "Biel Santamaría", "Leyre Vicente", "Alan Pascual",
                "Valentina Ballesteros", "Noah Reyes", "Martina Ferrer", "Lucas Aguilera", "Emma Figueroa",
                "Felipe Rojas", "Gabriela Soto", "Hernán Vidal", "Iris Mendoza", "Jaime Araya",
                "Karina Bustos", "Leonardo Cárdenas", "Marisol Duarte", "Nicolás Espinosa", "Olga Fuentes",
                "Patricio Godoy", "Quena Hidalgo", "Rodrigo Ibáñez", "Sonia Jara", "Tomás Krause",
                "Úrsula Lagos", "Vicente Morales", "Ximena Núñez", "Yerko Orellana", "Zoila Pinto",
                "Amelia Quiroz", "Benito Riquelme", "Camila Silva", "Dante Tapia", "Elsa Ulloa",
                "Fabián Valenzuela", "Gisela Yáñez", "Héctor Zambrano", "Ingrid Acevedo", "Jorge Briceño",
                "Karla Contreras", "Luciano Díaz", "Mónica Echeverría", "Néstor Fariña", "Olivia Guajardo",
                "Pablo Henríquez", "Renata Inostroza", "Simón Jiménez", "Tamara Keller", "Ulises Lizama"
            };

            string[] usernames = {
                "juanp", "mariag", "pedror", "anam", "luiss",
                "carmenl", "miguelf", "isabelg", "josed", "lauram",
                "carlosr", "sofiat", "antonioj", "elenar", "franciscon",
                "martaa", "davidg", "luciam", "javierd", "beatriza",
                "manuelv", "rosar", "alejandros", "cristinao", "andresc",
                "paular", "raulm", "silviao", "fernandam", "nuriad",
                "albertom", "raquelh", "robertom", "marinag", "angelc",
                "evap", "sergiov", "nataliac", "jorgef", "celiac",
                "danielc", "aliciai", "pabloc", "saral", "rubenr",
                "andreas", "adrianc", "teresag", "alvarol", "monicah",
                "diegov", "inesp", "ismaels", "estherc", "hugoa",
                "noeliav", "ivang", "lorenac", "victors", "patriciab",
                "eduardof", "auroram", "guillermog", "irenea", "enriquem",
                "rocioi", "tomasd", "yolandap", "samuelr", "veronicab",
                "marioh", "elisam", "marcoss", "lidias", "arturop",
                "miriamg", "ignacioe", "nerean", "gonzalof", "juliam",
                "ricardos", "dianav", "salvadorp", "soniab", "lorenzor",
                "carlaf", "jesusa", "taniab", "nicolash", "susanas",
                "joaquinh", "claudiad", "felipev", "ainaras", "rodrigop",
                "albam", "oscarm", "vanesac", "jaimeg", "ainhoas",
                "ramong", "larab", "ernestob", "noav", "emilioe",
                "carolinai", "julioc", "sandral", "borjas", "mireiaa",
                "alfredoc", "aitanao", "xavierr", "ariadnac", "ismaela",
                "carlotar", "estebanm", "judiths", "darior", "maro",
                "rafaelm", "irened", "joelm", "candelag", "aitorc",
                "abrilp", "marcv", "lolac", "brunof", "almac",
                "hectorp", "veras", "unaic", "adrianaa", "martinv",
                "danielag", "ikera", "valeriam", "paui", "vegad",
                "leop", "oliviar", "asierb", "jimenah", "mateom",
                "alexias", "fabiop", "leireg", "nile", "zoen",
                "alonsof", "chloem", "biels", "leyrev", "alanp",
                "valentinab", "noahr", "martinaf", "lucasa", "emmaf",
                "feliper", "gabrielas", "hernanv", "irism", "jaimea",
                "karinab", "leoc", "marisold", "nicolase", "olgaf",
                "patriciog", "quenah", "rodrigoi", "soniaj", "tomask",
                "ursulal", "vicentem", "ximenan", "yerkoo", "zoilap",
                "ameliaq", "benitor", "camilas", "dantet", "elsau",
                "fabianv", "giselay", "hectorzam", "ingrida", "jorgeb",
                "karlac", "lucianod", "monicae", "nestorf", "oliviag",
                "pabloh", "renatai", "simonj", "tamarak", "ulisesl"
            };

            // Superadmin
            var superadmin = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Superadminpro",
                NormalizedUserName = "SUPERADMINPRO",
                Email = "superadminpro@gmail.com",
                NormalizedEmail = "SUPERADMINPRO@GMAIL.COM",
                EmailConfirmed = true,
                Name = "Administrador de Plataforma",
                TenantId = "",
                Tmstmp = DateTime.Now,
                Saldo = 10000,
                Status = true,
            };
            superadmin.PasswordHash = hasher.HashPassword(superadmin, "superadmin123");
            users.Add(superadmin);
            userRoles.Add(new IdentityUserRole<string> { UserId = superadmin.Id, RoleId = "49aa311f-53c3-40a6-9ad4-11c7cb64fe4d" });

            // Regular Users (including AdminSitio)
            var random = new Random();
            var availableTenantIds = new List<string>(sites.Select(s => s.TenantId));

            for (int i = 0; i < 200; i++)
            {
                string tenantId;
                if (i < 10) // AdminSitio
                {
                    tenantId = availableTenantIds[i];
                }
                else // Usuario regular
                {
                    tenantId = availableTenantIds[random.Next(0, availableTenantIds.Count)];
                }

                var user = new AppUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = usernames[i],
                    NormalizedUserName = usernames[i].ToUpper(),
                    Email = $"{usernames[i].ToLower()}@gmail.com",
                    NormalizedEmail = $"{usernames[i].ToUpper()}@GMAIL.COM",
                    EmailConfirmed = true,
                    Name = nombres[i],
                    TenantId = tenantId,
                    Tmstmp = DateTime.Now,
                    Saldo = random.Next(0, 101),
                    Status = true,
                    RecibirNotificaciones = true
                };

                // Assign role and set password (first 10 are AdminSitio, rest are regular users)
                string roleId;
                string password;
                if (i < 10)
                {
                    roleId = "839311ef-70e2-4198-8607-0cd637dd385a";
                    password = "admin123";
                }
                else
                {
                    roleId = "adece3c4-4e32-4783-b5a7-fba4da010e9b";
                    password = "usuario123";
                }
                user.PasswordHash = hasher.HashPassword(user, password);
                users.Add(user);
                userRoles.Add(new IdentityUserRole<string> { UserId = user.Id, RoleId = roleId });
            }

            builder.Entity<AppUser>().HasData(users);
            builder.Entity<IdentityUserRole<string>>().HasData(userRoles);

            // Seed Pencas
            var campeonatos = new[]
            {
                "Liga Española", "Premier League", "Serie A", "Bundesliga", "Ligue 1",
                "Copa Los Amigos", "Europa League", "Copa Libertadores", "Copa Sudamericana", "Mundial FIFA",
                "Eurocopa", "Copa América", "Copa Africana de Naciones", "Liga MX", "MLS",
                "Eredivisie", "Primeira Liga", "Super Liga Argentina", "Brasileirão", "J1 League",
                "Liga Profesional Boliviana", "Primera División de Chile", "Categoría Primera A Colombia", "LigaPro Ecuador", "Liga Nacional de Guatemala",
                "Liga Nacional de Honduras", "Liga MX", "Primera División de Nicaragua", "Liga Panameña de Fútbol", "División Profesional Paraguay",
                "Liga 1 Perú", "Primera División de Uruguay", "Primera División de Venezuela", "A-League Australia", "Chinese Super League",
                "Indian Super League", "K League 1 Corea del Sur", "UAE Pro-League Emiratos Árabes", "Ligat ha'Al Israel", "Qatar Stars League",
                "Saudi Professional League", "Süper Lig Turquía", "Campeonato Catarinense", "Campeonato Paulista", "Campeonato Gaúcho",
                "Campeonato Mineiro", "Campeonato Carioca", "Copa del Rey", "FA Cup", "DFB-Pokal",
                "Coppa Italia", "Coupe de France", "KNVB Cup", "Taça de Portugal", "Copa Argentina",
                "Copa do Brasil", "US Open Cup", "Copa Colombia", "Copa Ecuador", "Copa Chile",
                "Recopa Sudamericana", "Mundial de Clubes", "Supercopa de Europa", "Community Shield", "Supercopa de España",
                "Supercoppa Italiana", "Trophée des Champions", "Johan Cruyff Shield", "Supertaça Cândido de Oliveira", "Supercopa Argentina"
            };

            var pencas = new List<Penca>();
            for (int i = 0; i < 70; i++)
            {
                pencas.Add(new Penca
                {
                    Id = Guid.NewGuid().ToString(),
                    Nombre = campeonatos[i],
                    FechaInicio = DateTime.Now.AddDays(10),
                    FechaFin = DateTime.Now.AddDays(30),
                    IsFinish = false
                });
            }
            builder.Entity<Penca>().HasData(pencas);

            // Seed UnoVsUno (EventoDeportivo)
            var eventos = new List<UnoVsUno>();
            string[] equipos = {
                "Barcelona", "Real Madrid", "Atlético Madrid", "Sevilla", "Valencia",
                "Manchester United", "Liverpool", "Chelsea", "Arsenal", "Manchester City",
                "Juventus", "Inter", "Milan", "Napoli", "Roma",
                "Bayern Munich", "Borussia Dortmund", "RB Leipzig", "Bayer Leverkusen", "Schalke 04",
                "PSG", "Lyon", "Marseille", "Monaco", "Lille",
                "Ajax", "PSV", "Feyenoord", "AZ Alkmaar", "Utrecht",
                "Benfica", "Porto", "Sporting CP", "Braga", "Vitória de Guimarães",
                "Boca Juniors", "River Plate", "Independiente", "Racing Club", "San Lorenzo",
                "Flamengo", "Palmeiras", "Santos", "São Paulo", "Corinthians"
            };

            for (int i = 1; i <= 200; i++)
            {
                var equipo1 = equipos[new Random().Next(equipos.Length)];
                var equipo2 = equipos[new Random().Next(equipos.Length)];
                while (equipo1 == equipo2)
                {
                    equipo2 = equipos[new Random().Next(equipos.Length)];
                }
                var diaRandom = new Random().Next(10, 30);
                eventos.Add(new UnoVsUno
                {
                    Id = Guid.NewGuid().ToString(),
                    Nombre = $"{equipo1} vs {equipo2}",
                    FechaInicio = DateTime.Now.AddDays(diaRandom),
                    FechaFin = DateTime.Now.AddDays(diaRandom),
                    EquipoUno = equipo1,
                    EquipoDos = equipo2,
                    HoraPartido = $"{diaRandom:D2}:00",
                    Canales = new[] { "ESPN", "Fox Sports" },
                    ScoreUno = "",
                    ScoreDos = "",
                    Deporte = "Fútbol",
                    ResultadoNotificado = false
                });
            }
            builder.Entity<UnoVsUno>().HasData(eventos);

            // Seed PencaSitio
            var pencasSitio = new List<PencaSitio>();

            for (int i = 1; i <= 10; i++) // Para cada sitio
            {
                for (int j = 1; j <= 6; j++) // 6 pencas por sitio
                {
                    var penca = pencas[random.Next(j - 1, pencas.Count)];

                    pencasSitio.Add(new PencaSitio
                    {
                        Id = Guid.NewGuid().ToString(),
                        SitioTenantId = availableTenantIds[i - 1],
                        PencaId = penca.Id,
                        Nombre = penca.Nombre,
                        Costo = random.Next(200, 501),
                        Premio = random.Next(1000, 5001),
                        Inscriptos = 0,
                        Comision = 20,
                        Recaudacion = 0
                    });
                }
            }
            builder.Entity<PencaSitio>().HasData(pencasSitio);

            // Seed Sistema
            builder.Entity<Sistema>().HasData(
                new Sistema
                {
                    Id = Guid.NewGuid().ToString(),
                    Comision = 20,
                    Billetera = 0,
                    TiempoNotificaciones = 5
                }
            );

            // Seed PencaEventos (tabla intermedia)
            var pencaEventos = new List<Dictionary<string, object>>();
            var eventosUsados = new HashSet<string>(); // Para llevar un registro de los eventos ya asignados

            for (int i = 0; i < 70; i++) // Para cada penca
            {
                var eventosDisponibles = new List<string>(eventos.Select(e => e.Id)); // Copia la lista de todos los eventos
                for (int j = 1; j <= 5; j++) // 5 eventos por penca
                {
                    if (eventosDisponibles.Count == 0)
                    {
                        // Si nos quedamos sin eventos, reiniciamos la lista
                        eventosDisponibles = new List<string>(eventos.Select(e => e.Id));
                    }

                    // Selecciona un evento aleatorio de los disponibles
                    var eventoIndex = random.Next(0, eventosDisponibles.Count);
                    var eventoId = eventosDisponibles[eventoIndex];

                    pencaEventos.Add(new Dictionary<string, object>
                    {
                        {"IdPenca", pencas[i].Id},
                        {"IdEventoDeportivo", eventoId}
                    });

                    // Elimina el evento usado de la lista de disponibles
                    eventosDisponibles.RemoveAt(eventoIndex);
                }
            }
            builder.Entity("PencaEvento").HasData(pencaEventos);

            // Seed PencaSitioUsuario
            var pencaSitioUsuarios = new List<PencaSitioUsuario>();

            foreach (var pencaSitio in pencasSitio)
            {
                // Obtiene usuarios que tienen el mismo TenantId que el SitioTenantId de la PencaSitio
                var usuariosDelMismoTenant = users.Where(u => u.TenantId == pencaSitio.SitioTenantId).ToList();
                if (usuariosDelMismoTenant.Count > 0)
                {
                    var usuariosAgregados = new HashSet<string>(); // Para rastrear usuarios ya agregados
                    var intentos = 0;
                    var maxIntentos = usuariosDelMismoTenant.Count * 2; // Para evitar bucle infinito

                    while (usuariosAgregados.Count < 10 && intentos < maxIntentos) // 10 usuarios únicos por cada PencaSitio
                    {
                        var usuario = usuariosDelMismoTenant[random.Next(usuariosDelMismoTenant.Count)];
                        if (!usuariosAgregados.Contains(usuario.Id))
                        {
                            usuariosAgregados.Add(usuario.Id);
                            pencaSitioUsuarios.Add(new PencaSitioUsuario
                            {
                                Id = Guid.NewGuid().ToString(),
                                IdPencaSitio = pencaSitio.Id,
                                IdUsuario = usuario.Id,
                                NombreUsuario = usuario.UserName,
                                Puntaje = 0,
                                Aciertos = 0
                            });
                            var addInscripcionPencaSitio = pencasSitio.FirstOrDefault(ps=>ps.Id == pencaSitio.Id);
                            addInscripcionPencaSitio.Inscriptos += 1;
                            addInscripcionPencaSitio.Recaudacion += addInscripcionPencaSitio.Costo - (addInscripcionPencaSitio.Costo * addInscripcionPencaSitio.Comision / 100);
                        }
                        intentos++;
                    }
                }
            }

            builder.Entity<PencaSitioUsuario>().HasData(pencaSitioUsuarios);

            // Seed Prediccion
            var predicciones = new List<Prediccion>();
            var combinacionesUsadas = new Dictionary<string, HashSet<string>>();
            int cantidadPredicciones = 2750;
            int prediccionesGeneradas = 0;

            while (prediccionesGeneradas < cantidadPredicciones)
            {
                foreach (var pencaSitioUsuario in pencaSitioUsuarios)
                {
                    var pencaSitio = pencasSitio.FirstOrDefault(ps => ps.Id == pencaSitioUsuario.IdPencaSitio);
                    if (pencaSitio == null)
                    {
                        continue;
                    }

                    var pencaEventosEspecificos = pencaEventos.Where(pe => pe["IdPenca"].ToString() == pencaSitio.PencaId).ToList();
                    if (pencaEventosEspecificos.Count == 0)
                    {
                        continue;
                    }

                    PencaSitioUsuario psu;
                    string eventoId;

                    do
                    {
                        psu = pencaSitioUsuarios[random.Next(pencaSitioUsuarios.Count)];
                        eventoId = pencaEventosEspecificos[random.Next(pencaEventosEspecificos.Count)]["IdEventoDeportivo"].ToString();
                    }
                    while (combinacionesUsadas.ContainsKey(psu.Id) && combinacionesUsadas[psu.Id].Contains(eventoId));

                    if (!combinacionesUsadas.ContainsKey(psu.Id))
                    {
                        combinacionesUsadas[psu.Id] = new HashSet<string>();
                    }
                    combinacionesUsadas[psu.Id].Add(eventoId);

                    predicciones.Add(new Prediccion
                    {
                        IdPencaSitioUsuario = psu.Id,
                        IdUnoVsUno = eventoId,
                        ScoreTeam1 = random.Next(0, 3),  // Puntuación aleatoria entre 0 y 2
                        ScoreTeam2 = random.Next(0, 3)   // Puntuación aleatoria entre 0 y 2
                    });

                    prediccionesGeneradas++;

                    if (prediccionesGeneradas >= cantidadPredicciones)
                    {
                        break;
                    }
                }

                // Reiniciar el bucle si aún no se han generado suficientes predicciones
                if (prediccionesGeneradas >= cantidadPredicciones)
                {
                    break;
                }
            }

            builder.Entity<Prediccion>().HasData(predicciones);


        }
    }
}
