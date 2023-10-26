using ConsoleApp1.DB;
using ConsoleApp1.Entitys;
using Microsoft.EntityFrameworkCore;

using var db = new Context();

bool endCLI = false;

while (!endCLI)
{
    string input = "";
    if (input is null) break;
    if (CLIHelper.IsInputEsc(input)) break;

    Console.WriteLine("O que deseja fazer (digite esc para sair)?");
    Console.WriteLine("1 - Cadastrar produto");
    Console.WriteLine("2 - Deletar produto");
    Console.WriteLine("3 - Atualizar estoque");
    Console.WriteLine("4 - Vizualizar produtos");
    Console.WriteLine("5 - Exibir produtos com baixo estoque");
    Console.WriteLine("");

    int option = 0;
    bool advance = false;
    while (!advance)
    {
        Console.Write("Digite sua opção: ");
        input = Console.ReadLine();

        if (CLIHelper.IsInputEsc(input)) break;

        try
        {
            option = Convert.ToInt32(input);
            if (option < 1 || option > 5) Console.WriteLine("**** Digite uma opção válida ****");
            else advance = true;
        }
        catch (Exception)
        {
            Console.WriteLine("**** Digite uma opção válida ****");
        }
    }

    advance = false;
    Console.WriteLine("\n");

    if (option == 1)
    {
        Console.Write("/******** Cadastrando Produto ********/\n");

        var product = new Product();

        Console.Write("Digite o nome do produto: ");
        input = Console.ReadLine();
        if (CLIHelper.IsInputEsc(input)) break;
        product.Name = input;

        Console.Write("Digite a categoria do produto: ");
        input = Console.ReadLine();
        if (CLIHelper.IsInputEsc(input)) break;
        product.Category = input;

        int quantity = 0;
        while (!advance)
        {
            Console.Write("Digite a quantidade do produto: ");
            input = Console.ReadLine();

            if (CLIHelper.IsInputEsc(input)) break;

            try
            {
                quantity = Convert.ToInt32(input);
                advance = true;
            }
            catch (Exception)
            {
                Console.WriteLine("**** Digite uma opção válida ****");
            }
        }
        advance = false;
        product.StockQuantity = quantity;

        decimal price = 0;
        while (!advance)
        {
            Console.Write("Digite o preço do produto: ");
            input = Console.ReadLine();

            if (CLIHelper.IsInputEsc(input)) break;

            try
            {
                price = Convert.ToDecimal(input);
                advance = true;
            }
            catch (Exception)
            {
                Console.WriteLine("**** Digite uma opção válida ****");
            }
        }
        advance = false;
        product.Price = price;

        try
        {
            db.Add(product);
            db.SaveChanges();
            Console.WriteLine("Produto adicionado com sucesso!");
        }
        catch (Exception)
        {
            Console.WriteLine("Erro ao adicionar produto!");
        }
    }
    else if (option == 2)
    {
        /* Deleta um produto*/
        var products = await db.Products
           .AsNoTracking()
           .ToArrayAsync();

        CLIHelper.DisplayProducts(products);

        var product = CLIHelper.GetProduct(products);

        Console.WriteLine($"\nVocê selecionou o produto: {product.Name}");
        bool confirmar = CLIHelper.IsContinue("Deseja deletar o produto");

        if (confirmar)
        {
            try
            {
                db.Remove(product);
                db.SaveChanges();
                Console.WriteLine("Produto deletado com sucesso!");
            }
            catch (Exception)
            {
                Console.WriteLine("Erro ao deletar produto!");
            }
        }
    }
    else if (option == 3)
    {
        /* Atualiza a quantidade de produtos em estoque
           a partir da quantidade de produtos vendidos*/

        var products = await db.Products
           .AsNoTracking()
           .ToArrayAsync();

        CLIHelper.DisplayProducts(products);

        int IdSelected = 0;
        var product = CLIHelper.GetProduct(products);

        int quantitySold = 0;
        while (!advance)
        {
            Console.Write("\nQuantos produtos foram vendidos? ");
            input = Console.ReadLine();
            if (CLIHelper.IsInputEsc(input)) break;

            try
            {
                quantitySold = Convert.ToInt32(input);
                advance = true;
            }
            catch (Exception)
            {
                Console.WriteLine("**** Digite uma opção válida ****");
            }
        }

        advance = false;
        product.StockQuantity -= quantitySold;

        try
        {
            db.Update(product);
            db.SaveChanges();
            Console.WriteLine("Produto atualizado!");
        }
        catch (Exception)
        {
            Console.WriteLine("Erro ao atualizar produto!");
        }


    }
    else if (option == 4)
    {
        /* Mostra todos os produtos */
        var products = await db.Products
            .AsNoTracking()
            .ToArrayAsync();

        CLIHelper.DisplayProducts(products);
    }
    else if (option == 5)
    {
        /* Mostra todos os produtos
           que tem menos de 10 unidades
           em estoque*/
        var products = await db.Products
            .AsNoTracking()
            .Where(x => x.StockQuantity < 10)
            .ToArrayAsync();

        CLIHelper.DisplayProducts(products);
    }

    Console.WriteLine("\n");
    bool IsContinue = CLIHelper.IsContinue("Deseja continuar no programa");

    Console.Clear();
    if (!IsContinue) endCLI = true;
}

public static class CLIHelper
{
    /* Esse método verifica se a entrada digitada é esc */
    public static bool IsInputEsc(string input)
    {
        if (input.Equals("esc", StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }

    public static void DisplayProducts(Product[] products)
    {
        Console.WriteLine($"\t Nome | Categoria | Quantidade | Preço");
        Console.WriteLine($"********************************************");
        Console.WriteLine($"--------------------------------------------");
        foreach (var product in products)
        {
            Console.WriteLine($"{product.Id} \t {product.Name} | {product.Category} | {product.StockQuantity} | {product.Price}");
            Console.WriteLine($"____________________________________________");
        }
    }

    public static Product GetProduct(Product[] products)
    {
        int IdSelected = 0;
        bool advance = false;
        string input = "";
        while (!advance)
        {
            Console.Write("Selecione um produto: ");
            input = Console.ReadLine();

            if (IsInputEsc(input)) break;

            try
            {
                IdSelected = Convert.ToInt32(input);
                if (products.Select(x => x.Id).Contains(IdSelected))
                    advance = true;
                else
                    Console.WriteLine("**** Digite uma opção válida ****");
            }
            catch (Exception)
            {
                Console.WriteLine("**** Digite uma opção válida ****");
            }
        }

        return products.Where(x => x.Id == Convert.ToInt32(input)).First();
    }

    public static bool IsContinue(string consoleMessage)
    {
        bool IsContinue = false;
        bool stopLoop = false;
        while (!stopLoop)
        {
            Console.Write($"{consoleMessage} (S/N)? ");
            var input = Console.ReadLine();
            if (input.Equals("S", StringComparison.OrdinalIgnoreCase) || input.Equals("N", StringComparison.OrdinalIgnoreCase))
            {
                if (input.Equals("S", StringComparison.OrdinalIgnoreCase)) IsContinue = true;
                else IsContinue = false;
                stopLoop = true;
            }
            else
                Console.WriteLine("Entrada inválida! Por favor, insira apenas 'S' ou 'N'.");
        }

        return IsContinue;
    }
}
