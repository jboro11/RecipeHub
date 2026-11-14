using RecipeHub.Domain.Entities;
using RecipeHub.Domain.Models;

namespace RecipeHub.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Základní kategorie musí být dostupné i bez ukázkových receptů.
            var categoryNames = new[] { "Snídaně", "Hlavní jídlo", "Dezert", "Polévky", "Nápoje" };
            bool changesMade = false;

            foreach (var name in categoryNames)
            {
                // Kontrola, zda kategorie už v DB není
                if (!context.Categories.Any(c => c.Name == name))
                {
                    context.Categories.Add(new Category { Name = name });
                    changesMade = true;
                }
            }

            if (changesMade)
            {
                context.SaveChanges();
            }

            if (context.Recipes.Any()) return;

            // Kategorie už mají přidělené databázové ID.
            var categories = context.Categories.ToList();
            
            // Pokud se z nějakého důvodu kategorie nepodařilo načíst, skončíme
            if (!categories.Any()) return; 

            var catBreakfast = categories.FirstOrDefault(c => c.Name == "Snídaně");
            var catMain = categories.FirstOrDefault(c => c.Name == "Hlavní jídlo");
            var catDessert = categories.FirstOrDefault(c => c.Name == "Dezert");
            var catSoup = categories.FirstOrDefault(c => c.Name == "Polévky");
            var catDrinks = categories.FirstOrDefault(c => c.Name == "Nápoje");

            // 2. Definice receptů
            var recipes = new Recipe[]
            {
                new Recipe
                {
                    Title = "Svíčková na smetaně",
                    ShortDescription = "Tradiční česká klasika s knedlíkem.",
                    Description = "Maso prošpikujeme špekem, osolíme, opepříme. Na sádle orestujeme kořenovou zeleninu, přidáme cibuli, koření a maso zatáhneme. Podlijeme vodou a dusíme doměkka. Maso vyjmeme, zeleninu rozmixujeme, přidáme smetanu a dochutíme citronem a cukrem. Podáváme s houskovým knedlíkem.",
                    Category = "Hlavní jídlo",
                    CategoryNavigation = catMain, // Vazba na entitu
                    PrepTime = "2 h",
                    Difficulty = 3,
                    ImageUrl = "https://cdn.administrace.tv/2022/08/06/hd/f1f13d31984ad04ed5bd24d501678a2d.jpg",
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Name = "Hovězí zadní", Quantity = "800g" },
                        new Ingredient { Name = "Mrkev, celer, petržel", Quantity = "500g" },
                        new Ingredient { Name = "Smetana ke šlehání", Quantity = "250ml" },
                        new Ingredient { Name = "Houskový knedlík", Quantity = "1 balení" }
                    }
                },
                new Recipe
                {
                    Title = "Míchaná vajíčka",
                    ShortDescription = "Rychlá a výživná snídaně.",
                    Description = "Na pánvi rozpustíme máslo. Přidáme rozšlehaná vejce se solí a pepřem. Mícháme na mírném ohni, dokud vejce nezhoustnou, ale jsou stále vláčná. Posypeme pažitkou.",
                    Category = "Snídaně",
                    CategoryNavigation = catBreakfast,
                    PrepTime = "15 minut",
                    Difficulty = 1,
                    ImageUrl = "https://cdn.administrace.tv/2022/08/07/hd/61e43de497551d16477812beaebd3e5c.jpg",
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Name = "Vejce", Quantity = "3 ks" },
                        new Ingredient { Name = "Máslo", Quantity = "1 lžíce" },
                        new Ingredient { Name = "Sůl, pepř", Quantity = "dle chuti" },
                        new Ingredient { Name = "Pažitka", Quantity = "na ozdobu" }
                    }
                },
                new Recipe
                {
                    Title = "Čokoládový dort",
                    ShortDescription = "Luxusní dezert pro milovníky čokolády.",
                    Description = "Rozpustíme čokoládu s máslem. Vyšleháme vejce s cukrem do pěny, přidáme mouku a čokoládovou směs. Pečeme ve formě cca 30 minut na 180°C. Necháme vychladnout a potřeme polevou.",
                    Category = "Dezert",
                    CategoryNavigation = catDessert,
                    PrepTime = "1 h",
                    Difficulty = 2,
                    ImageUrl = "https://images.unsplash.com/photo-1578985545062-69928b1d9587?auto=format&fit=crop&w=800&q=80",
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Name = "Hořká čokoláda", Quantity = "200g" },
                        new Ingredient { Name = "Máslo", Quantity = "150g" },
                        new Ingredient { Name = "Vejce", Quantity = "4 ks" },
                        new Ingredient { Name = "Cukr", Quantity = "100g" },
                        new Ingredient { Name = "Hladká mouka", Quantity = "80g" }
                    }
                },
                new Recipe
                {
                    Title = "Gulášová polévka",
                    ShortDescription = "Poctivá a sytá polévka.",
                    Description = "Na sádle osmahneme cibuli, přidáme maso nakrájené na kostičky, papriku a zalijeme vývarem. Přidáme brambory a vaříme doměkka. Zahustíme jíškou, dochutíme česnekem a majoránkou.",
                    Category = "Polévky",
                    CategoryNavigation = catSoup,
                    PrepTime = "45 minut",
                    Difficulty = 2,
                    ImageUrl = "https://images.unsplash.com/photo-1547592166-23ac45744acd?auto=format&fit=crop&w=800&q=80",
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Name = "Hovězí maso", Quantity = "300g" },
                        new Ingredient { Name = "Brambory", Quantity = "3 ks" },
                        new Ingredient { Name = "Cibule", Quantity = "1 ks" },
                        new Ingredient { Name = "Mletá paprika", Quantity = "1 lžíce" }
                    }
                },
                new Recipe
                {
                    Title = "Spaghetti Carbonara",
                    ShortDescription = "Italská klasika se slaninou a vejci.",
                    Description = "Uvaříme špagety al dente. Na pánvi opečeme guanciale (nebo pancettu). V misce smícháme žloutky s pecorinem a pepřem. Špagety smícháme s tukem ze slaniny, odstavíme z ohně a vmícháme vaječnou směs s trochou vody z těstovin, aby vznikla krémová omáčka.",
                    Category = "Hlavní jídlo",
                    CategoryNavigation = catMain,
                    PrepTime = "30 minut",
                    Difficulty = 2,
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/3/33/Espaguetis_carbonara.jpg/800px-Espaguetis_carbonara.jpg",
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Name = "Špagety", Quantity = "400g" },
                        new Ingredient { Name = "Guanciale/Slanina", Quantity = "150g" },
                        new Ingredient { Name = "Vaječné žloutky", Quantity = "4 ks" },
                        new Ingredient { Name = "Pecorino Romano", Quantity = "100g" }
                    }
                },
                new Recipe
                {
                    Title = "Caesar Salát",
                    ShortDescription = "Svěží salát s krutony a parmazánem.",
                    Description = "Římský salát natrháme. Připravíme dresink z ančoviček, žloutků, oleje, citronu a parmazánu. Smícháme se salátem, posypeme krutony a hoblinami parmazánu. Můžeme přidat grilované kuřecí maso.",
                    Category = "Hlavní jídlo",
                    CategoryNavigation = catMain,
                    PrepTime = "30 minut",
                    Difficulty = 1,
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/23/Caesar_salad_%282%29.jpg/800px-Caesar_salad_%282%29.jpg",
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Name = "Římský salát", Quantity = "1 ks" },
                        new Ingredient { Name = "Parmazán", Quantity = "50g" },
                        new Ingredient { Name = "Krutony", Quantity = "hrst" },
                        new Ingredient { Name = "Kuřecí prsa", Quantity = "200g" }
                    }
                },
                new Recipe
                {
                    Title = "Dýňová polévka",
                    ShortDescription = "Krémová podzimní polévka.",
                    Description = "Dýni hokaido nakrájíme a orestujeme na cibulce. Zalijeme vývarem a vaříme doměkka. Rozmixujeme, zjemníme smetanou a dochutíme muškátovým oříškem. Podáváme s dýňovými semínky.",
                    Category = "Polévky",
                    CategoryNavigation = catSoup,
                    PrepTime = "45 minut",
                    Difficulty = 2,
                    ImageUrl = "https://images.unsplash.com/photo-1476718406336-bb5a9690ee2a?q=80&w=800&auto=format&fit=crop",
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Name = "Dýně Hokaido", Quantity = "1 ks" },
                        new Ingredient { Name = "Vývar", Quantity = "1 l" },
                        new Ingredient { Name = "Smetana", Quantity = "200ml" },
                        new Ingredient { Name = "Cibule", Quantity = "1 ks" }
                    }
                },
                new Recipe
                {
                    Title = "Lívance s borůvkami",
                    ShortDescription = "Nadýchané lívance pro sladké ráno.",
                    Description = "Smícháme mouku, prášek do pečiva, cukr a sůl. Přidáme mléko, vejce a rozpuštěné máslo. Smažíme na pánvi dozlatova. Podáváme s borůvkami a javorovým sirupem.",
                    Category = "Snídaně",
                    CategoryNavigation = catBreakfast,
                    PrepTime = "30 minut",
                    Difficulty = 2,
                    ImageUrl = "https://images.unsplash.com/photo-1528207776546-365bb710ee93?q=80&w=800&auto=format&fit=crop",
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Name = "Hladká mouka", Quantity = "200g" },
                        new Ingredient { Name = "Mléko", Quantity = "250ml" },
                        new Ingredient { Name = "Vejce", Quantity = "1 ks" },
                        new Ingredient { Name = "Borůvky", Quantity = "100g" }
                    }
                },
                new Recipe
                {
                    Title = "Tiramisu",
                    ShortDescription = "Nepečený italský dezert s kávou.",
                    Description = "Žloutky vyšleháme s cukrem, přidáme mascarpone. Cukrářské piškoty namáčíme v silné kávě s Amarettem a skládáme do formy. Střídáme vrstvy krému a piškotů. Posypeme kakaem a necháme ztuhnout.",
                    Category = "Dezert",
                    CategoryNavigation = catDessert,
                    PrepTime = "45 minut",
                    Difficulty = 3,
                    ImageUrl = "https://www.micheldumas.com/wp-content/uploads/2025/05/tiramisu1-ezgif.com-jpg-to-webp-converter-1-1.webp",
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Name = "Mascarpone", Quantity = "500g" },
                        new Ingredient { Name = "Vejce", Quantity = "4 ks" },
                        new Ingredient { Name = "Cukrářské piškoty", Quantity = "1 balení" },
                        new Ingredient { Name = "Silná káva", Quantity = "200ml" },
                        new Ingredient { Name = "Kakao", Quantity = "na posypání" }
                    }
                },
                new Recipe
                {
                    Title = "Hovězí Burger",
                    ShortDescription = "Domácí burger s čedarem.",
                    Description = "Mleté maso ochutíme solí a pepřem, vytvarujeme placky a grilujeme. Bulky rozkrojíme a opečeme. Skládáme: dresink, salát, maso, sýr, rajče, cibule. Podáváme s hranolky.",
                    Category = "Hlavní jídlo",
                    CategoryNavigation = catMain,
                    PrepTime = "45 minut",
                    Difficulty = 2,
                    ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?q=80&w=800&auto=format&fit=crop",
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Name = "Mleté hovězí", Quantity = "500g" },
                        new Ingredient { Name = "Bulky na burger", Quantity = "4 ks" },
                        new Ingredient { Name = "Čedar", Quantity = "4 plátky" },
                        new Ingredient { Name = "Rajče, salát, cibule", Quantity = "dle chuti" }
                    }
                },
                new Recipe
                {
                    Title = "Ovesná kaše",
                    ShortDescription = "Zdravá snídaně plná energie.",
                    Description = "Vločky vaříme v mléce se špetkou soli do zhoustnutí. Osladíme medem, přidáme ořechy a ovoce dle chuti.",
                    Category = "Snídaně",
                    CategoryNavigation = catBreakfast,
                    PrepTime = "15 minut",
                    Difficulty = 1,
                    ImageUrl = "https://foodbyjonister.com/wp-content/uploads/2022/01/ChocolateNutOatmeal4-879x1300.jpg",
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Name = "Ovesné vločky", Quantity = "50g" },
                        new Ingredient { Name = "Mléko", Quantity = "250ml" },
                        new Ingredient { Name = "Med", Quantity = "1 lžíce" },
                        new Ingredient { Name = "Ořechy", Quantity = "hrst" }
                    }
                },
                new Recipe
                {
                    Title = "Kulajda",
                    ShortDescription = "Tradiční jihočeská polévka s houbami.",
                    Description = "Brambory uvaříme s kmínem a houbami. Zahustíme smetanou s moukou, přidáme kopr a dochutíme octem a cukrem. Podáváme se ztraceným vejcem.",
                    Category = "Polévky",
                    CategoryNavigation = catSoup,
                    PrepTime = "45 minut",
                    Difficulty = 2,
                    ImageUrl = "https://gymbeam.cz/blog/wp-content/uploads/2025/10/Navrh-bez-nazvu-2025-10-27T073627.061.png",
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Name = "Brambory", Quantity = "4 ks" },
                        new Ingredient { Name = "Houby (sušené/čerstvé)", Quantity = "hrst" },
                        new Ingredient { Name = "Smetana", Quantity = "200ml" },
                        new Ingredient { Name = "Kopr", Quantity = "svazek" },
                        new Ingredient { Name = "Vejce", Quantity = "4 ks" }
                    }
                },
                new Recipe
                {
                    Title = "Domácí limonáda",
                    ShortDescription = "Citronová limonáda pro horké dny.",
                    Description = "Vymačkáme citrony, smícháme s cukrovým sirupem a dolijeme vodou a ledem. Přidáme plátky citronu.",
                    Category = "Nápoje",
                    CategoryNavigation = catDrinks,
                    PrepTime = "15 minut",
                    Difficulty = 1,
                    ImageUrl = "https://images.unsplash.com/photo-1621263764928-df1444c5e859?q=80&w=800&auto=format&fit=crop",
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Name = "Citrony", Quantity = "3 ks" },
                        new Ingredient { Name = "Cukr", Quantity = "100g" },
                        new Ingredient { Name = "Voda", Quantity = "1 l" },
                        new Ingredient { Name = "Led", Quantity = "hodně" }
                    }
                },
                new Recipe
                {
                    Title = "Lasagne",
                    ShortDescription = "Zapékané těstoviny s boloňskou omáčkou.",
                    Description = "Připravíme boloňskou omáčku z mletého masa a rajčat. Připravíme bešamel. Do pekáče vrstvíme: lasagne, boloňskou omáčku, bešamel, parmazán. Opakujeme. Pečeme cca 40 min.",
                    Category = "Hlavní jídlo",
                    CategoryNavigation = catMain,
                    PrepTime = "1,5 h",
                    Difficulty = 3,
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/ba/Lasagne_-_stonesoup.jpg/800px-Lasagne_-_stonesoup.jpg",
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Name = "Mleté maso", Quantity = "500g" },
                        new Ingredient { Name = "Pláty lasagní", Quantity = "1 balení" },
                        new Ingredient { Name = "Rajčatová passata", Quantity = "500ml" },
                        new Ingredient { Name = "Mléko, máslo, mouka", Quantity = "na bešamel" }
                    }
                },
                new Recipe
                {
                    Title = "Bábovka",
                    ShortDescription = "Nedělní mramorová bábovka.",
                    Description = "Vyšleháme vejce s cukrem, přidáme olej, mléko a mouku s práškem do pečiva. Část těsta obarvíme kakaem. Do formy střídavě lijeme světlé a tmavé těsto. Pečeme na 180°C cca 45-50 min.",
                    Category = "Dezert",
                    CategoryNavigation = catDessert,
                    PrepTime = "1 h",
                    Difficulty = 2,
                    ImageUrl = "https://www.babiccinavolba.cz/wp-content/uploads/2019/08/mramorova_babovka.jpg",
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Name = "Polohrubá mouka", Quantity = "300g" },
                        new Ingredient { Name = "Cukr krupice", Quantity = "200g" },
                        new Ingredient { Name = "Vejce", Quantity = "4 ks" },
                        new Ingredient { Name = "Olej", Quantity = "100ml" },
                        new Ingredient { Name = "Kakao", Quantity = "2 lžíce" }
                    }
                },
                new Recipe
                {
                    Title = "Řecký salát",
                    ShortDescription = "Salát s fetou a olivami.",
                    Description = "Zeleninu nakrájíme na větší kusy. Přidáme olivy a plátek sýra Feta. Zakápneme olivovým olejem a posypeme oreganem.",
                    Category = "Hlavní jídlo",
                    CategoryNavigation = catMain,
                    PrepTime = "15 minut",
                    Difficulty = 1,
                    ImageUrl = "https://images.unsplash.com/photo-1540189549336-e6e99c3679fe?q=80&w=800&auto=format&fit=crop",
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Name = "Rajčata, okurka, paprika", Quantity = "dle chuti" },
                        new Ingredient { Name = "Sýr Feta", Quantity = "200g" },
                        new Ingredient { Name = "Černé olivy", Quantity = "hrst" },
                        new Ingredient { Name = "Olivový olej", Quantity = "2 lžíce" }
                    }
                },
                new Recipe
                {
                    Title = "Česnečka",
                    ShortDescription = "Silná česneková polévka.",
                    Description = "Do vývaru přidáme brambory a vaříme. Ke konci přidáme prolisovaný česnek a majoránku. Podáváme s opečeným chlebem a sýrem.",
                    Category = "Polévky",
                    CategoryNavigation = catSoup,
                    PrepTime = "30 minut",
                    Difficulty = 1,
                    ImageUrl = "https://images.unsplash.com/photo-1547592180-85f173990554?q=80&w=800&auto=format&fit=crop",
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Name = "Vývar", Quantity = "1 l" },
                        new Ingredient { Name = "Brambory", Quantity = "3 ks" },
                        new Ingredient { Name = "Česnek", Quantity = "6 stroužků" },
                        new Ingredient { Name = "Chléb, sýr", Quantity = "k podávání" }
                    }
                },
                new Recipe
                {
                    Title = "Cappuccino",
                    ShortDescription = "Káva s našlehaným mlékem.",
                    Description = "Připravíme espresso. Mléko našleháme tryskou na mikropěnu. Nalijeme do espressa tak, aby vznikla čepice pěny.",
                    Category = "Nápoje",
                    CategoryNavigation = catDrinks,
                    PrepTime = "15 minut",
                    Difficulty = 2,
                    ImageUrl = "https://images.unsplash.com/photo-1572442388796-11668a67e53d?q=80&w=800&auto=format&fit=crop",
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Name = "Káva (espresso)", Quantity = "30ml" },
                        new Ingredient { Name = "Plnotučné mléko", Quantity = "150ml" }
                    }
                },
                new Recipe
                {
                    Title = "Steak s rozmarínom",
                    ShortDescription = "Šťavnatý rib-eye steak marinovaný v bylinkovom masle podávaný s pečenými zemiakmi.",
                    Description = "Rib-eye steak osolíme, opepříme a opečeme na pánvi s rozmarýnem a máslem. Podáváme s pečenými bramborami.",
                    Category = "Hlavní jídlo",
                    CategoryNavigation = catMain,
                    PrepTime = "45 min",
                    Difficulty = 3,
                    ImageUrl = "https://images.unsplash.com/photo-1600891964092-4316c288032e?ixlib=rb-1.2.1&auto=format&fit=crop&w=600&q=80",
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Name = "Rib-eye steak", Quantity = "200g" },
                        new Ingredient { Name = "Máslo", Quantity = "50g" },
                        new Ingredient { Name = "Rozmarýn", Quantity = "snítka" },
                        new Ingredient { Name = "Brambory", Quantity = "200g" }
                    }
                },
                new Recipe
                {
                    Title = "Pikantné kari",
                    ShortDescription = "Indické krémové kari s kokosovým mliekom, chilli a čerstvým koriandrom.",
                    Description = "Na oleji orestujeme kari pastu, přidáme maso/cizrnu a zalijeme kokosovým mlékem. Vaříme do zhoustnutí. Dochutíme chilli a koriandrem. Podáváme s rýží.",
                    Category = "Hlavní jídlo",
                    CategoryNavigation = catMain,
                    PrepTime = "30 min",
                    Difficulty = 1,
                    ImageUrl = "https://images.unsplash.com/photo-1565557623262-b51c2513a641?ixlib=rb-1.2.1&auto=format&fit=crop&w=600&q=80",
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Name = "Kuřecí maso", Quantity = "150g" },
                        new Ingredient { Name = "Kokosové mléko", Quantity = "200ml" },
                        new Ingredient { Name = "Kari pasta", Quantity = "1 lžíce" },
                        new Ingredient { Name = "Rýže", Quantity = "150g" }
                    }
                },
                new Recipe
                {
                    Title = "Hľuzovkové cestoviny",
                    ShortDescription = "Domáce tagliatelle s jemnou smotanovo-hľuzovkovou omáčkou a parmezánom.",
                    Description = "Uvaříme tagliatelle. Na pánvi zahřejeme smetanu s lanýžovou pastou/olejem. Promícháme s těstovinami a posypeme parmazánem.",
                    Category = "Hlavní jídlo",
                    CategoryNavigation = catMain,
                    PrepTime = "20 min",
                    Difficulty = 2,
                    ImageUrl = "https://daybyme.s3.eu-west-2.amazonaws.com/items/1562505236BQ3E3yjwHp.jpg",
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient { Name = "Tagliatelle", Quantity = "150g" },
                        new Ingredient { Name = "Smetana", Quantity = "100ml" },
                        new Ingredient { Name = "Lanýžová pasta", Quantity = "1 lžička" },
                        new Ingredient { Name = "Parmazán", Quantity = "30g" }
                    }
                }
            };

            // Ukázkové recepty vložíme pouze do prázdné databáze.
            if (!context.Recipes.Any())
            {
                context.Recipes.AddRange(recipes);
                context.SaveChanges();
            }
        }
    }
}
