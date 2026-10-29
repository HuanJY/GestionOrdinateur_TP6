namespace GestionOrdinateurs.API;

// gestion du stock de pieces
public class StockService
{
    // stock de SSD dispo 
    public int StockSSD { get; set; } = 10;

    //verrou
    private readonly object _stockLock = new object();

    // consomme 1 SSD : délai, puis diminution du stock de 1 
    public void ConsommerSSD()
    {
        Task.Delay(10).Wait();   // délai avant de diminuer le stock

        lock (_stockLock)
        {
            StockSSD--;          //  un thread a la fois 
        }
    }
}



//Q.20 grace au verrou, le resultat de stock de ssd est toujours égal à 0, ce qui est attendu  