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
    Console.WriteLine("2 - Vizualizar produtos");
    Console.WriteLine("3 - Atualizar estoque");
    Console.WriteLine("4 - Exibir produtos com baixo estoque\n");

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
            if (option < 1 || option > 4) Console.WriteLine("**** Digite uma opção válida ****");
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

        Console.Write("Digite o preço do produto: ");
        input = Console.ReadLine();
        if (CLIHelper.IsInputEsc(input)) break;

        decimal price = 0;
        while (!advance)
        {
            Console.Write("Digite a quantidade do produto: ");
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
        product.Price = Convert.ToDecimal(input);

        db.Add(product);
        db.SaveChanges();

        var produto = await db.Products
            .AsNoTracking()
            .Where(x => x.Name == product.Name)
            .FirstAsync();

        Console.WriteLine(produto.Name);
    }
    else if (option == 2)
    {
        var products = await db.Products
            .AsNoTracking()
            .ToArrayAsync();

        CLIHelper.DisplayProducts(products);
    }
    else if (option == 3)
    {
        var products = await db.Products
           .AsNoTracking()
           .ToArrayAsync();

        CLIHelper.DisplayProducts(products);
        Console.Write("Selecione um produto: ");
        input = Console.ReadLine();
        if (CLIHelper.IsInputEsc(input)) break;

        var product = products.Where(x => x.Id == Convert.ToInt32(input)).First();

        Console.WriteLine($"Há {product.StockQuantity} prdutos no estoque");
       
        int quantitySold = 0;
        while (!advance)
        {
            Console.Write("Quantos produtos foram vendidos? ");
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

        db.Update(product);
        db.SaveChanges();

    }
    else if (option == 4)
    {
        var products = await db.Products
            .AsNoTracking()
            .Where(x => x.StockQuantity < 10)
            .ToArrayAsync();

        CLIHelper.DisplayProducts(products);
    }

    Console.WriteLine("\n");
    bool IsContinue = false;
    while (!IsContinue)
    {
        Console.Write("Deseja fazer mais alguma coisa (s/N)? ");
        input = Console.ReadLine();
        if (input is null || input.Length > 1)
            Console.WriteLine("Entrada inválida! Por favor, insira apenas 'y' ou 'n'.");
        else
            IsContinue = true;
    }

    Console.Clear();
    if (input.Equals("n", StringComparison.OrdinalIgnoreCase)) endCLI = true;
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
        foreach (var product in products)
        {
            Console.WriteLine($"{product.Id} \t {product.Name} | {product.Category} | {product.StockQuantity} | {product.Price}");
            Console.WriteLine($"____________________________________________");
        }
    }

}
