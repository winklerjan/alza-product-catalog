using ProductCatalog.Domain;

namespace ProductCatalog.Infrastructure.Seed;

public static class ProductSeedData
{
    public static IReadOnlyList<Product> All { get; } =
    [
        new(1, "Apple MacBook Air 13\" M4", "https://placehold.co/600x400?text=MacBook+Air", 29990.00m,
            "Thin and light laptop with all-day battery life.", isAvailable: true),
        new(2, "Dell XPS 13", "https://placehold.co/600x400?text=Dell+XPS+13", 27490.00m,
            "Compact Windows ultrabook with a 13-inch InfinityEdge display.", isAvailable: true),
        new(3, "Lenovo ThinkPad X1 Carbon", "https://placehold.co/600x400?text=ThinkPad+X1", 34990.00m, null,
            isAvailable: false),
        new(4, "ASUS ROG Zephyrus G14", "https://placehold.co/600x400?text=ROG+Zephyrus", 42990.00m,
            "14-inch gaming laptop with a dedicated RTX GPU.", isAvailable: true),
        new(5, "HP Spectre x360", "https://placehold.co/600x400?text=Spectre+x360", 31990.00m,
            "Convertible laptop with a 360-degree hinge and OLED display.", isAvailable: true),
        new(6, "Apple iPhone 16", "https://placehold.co/600x400?text=iPhone+16", 23990.00m,
            "Flagship smartphone with the A18 chip and dual camera system.", isAvailable: true),
        new(7, "Samsung Galaxy S25", "https://placehold.co/600x400?text=Galaxy+S25", 21990.00m, null,
            isAvailable: true),
        new(8, "Google Pixel 9", "https://placehold.co/600x400?text=Pixel+9", 18990.00m,
            "Stock Android phone with computational photography.", isAvailable: true),
        new(9, "Samsung Galaxy A55", "https://placehold.co/600x400?text=Galaxy+A55", 8990.00m,
            "Mid-range smartphone with a 120Hz AMOLED display.", isAvailable: true),
        new(10, "Apple iPhone SE", "https://placehold.co/600x400?text=iPhone+SE", 12990.00m, null,
            isAvailable: true),
        new(11, "Sony WH-1000XM5", "https://placehold.co/600x400?text=Sony+WH-1000XM5", 8490.00m,
            "Over-ear noise-cancelling headphones with 30-hour battery life.", isAvailable: true),
        new(12, "Apple AirPods Pro 2", "https://placehold.co/600x400?text=AirPods+Pro+2", 6490.00m,
            "In-ear earbuds with active noise cancellation.", isAvailable: true),
        new(13, "Bose QuietComfort Ultra", "https://placehold.co/600x400?text=Bose+QC+Ultra", 9490.00m, null,
            isAvailable: false),
        new(14, "Sennheiser Momentum 4", "https://placehold.co/600x400?text=Momentum+4", 7990.00m,
            "Audiophile-tuned wireless headphones with a 60-hour battery.", isAvailable: true),
        new(15, "JBL Tune 760NC", "https://placehold.co/600x400?text=JBL+Tune+760NC", 1990.00m,
            "Budget-friendly noise-cancelling headphones.", isAvailable: true),
        new(16, "LG UltraGear 27GP950", "https://placehold.co/600x400?text=UltraGear+27GP950", 15990.00m,
            "27-inch 4K gaming monitor with a 144Hz refresh rate.", isAvailable: true),
        new(17, "Dell UltraSharp U2723QE", "https://placehold.co/600x400?text=UltraSharp+U2723QE", 13990.00m, null,
            isAvailable: true),
        new(18, "Samsung Odyssey G7", "https://placehold.co/600x400?text=Odyssey+G7", 12990.00m,
            "32-inch curved gaming monitor with a 240Hz refresh rate.", isAvailable: true),
        new(19, "ASUS ProArt PA278CV", "https://placehold.co/600x400?text=ProArt+PA278CV", 9990.00m,
            "27-inch monitor calibrated for color-accurate work.", isAvailable: true),
        new(20, "BenQ GW2780", "https://placehold.co/600x400?text=BenQ+GW2780", 4490.00m, null, isAvailable: false),
        new(21, "Logitech MX Master 3S", "https://placehold.co/600x400?text=MX+Master+3S", 2490.00m,
            "Ergonomic wireless mouse with an 8K DPI sensor.", isAvailable: true),
        new(22, "Keychron K8 Pro", "https://placehold.co/600x400?text=Keychron+K8+Pro", 3290.00m,
            "Hot-swappable mechanical keyboard with wireless connectivity.", isAvailable: true),
        new(23, "Logitech C920 HD Pro", "https://placehold.co/600x400?text=Logitech+C920", 1790.00m, null,
            isAvailable: true),
        new(24, "Anker PowerCore 20000", "https://placehold.co/600x400?text=PowerCore+20000", 1290.00m,
            "20,000mAh portable charger with fast charging support.", isAvailable: true),
        new(25, "Apple Watch Series 10", "https://placehold.co/600x400?text=Apple+Watch+S10", 11990.00m,
            "Smartwatch with health tracking and always-on display.", isAvailable: true)
    ];
}
