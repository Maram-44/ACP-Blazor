using Microsoft.AspNetCore.Components;
using ACP.Services;
using ACP.Models.Products;

namespace ACP.Pages;

public partial class Cart
{
    [Inject] private BasketService BasketService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    public List<ItemsBasket>? CartItems { get; set; }

    private decimal subtotal;
    private decimal shipping = 10.00m;
    private int currentCustomerId = 1;

    protected override async Task OnInitializedAsync()
    {
        await LoadCartData();
    }

    //private async Task LoadCartData()
    //{
    //    try
    //    {
    //        var result = await BasketService.GetCustomerBasketAsync(currentCustomerId);
    //        if (result != null)
    //        {
    //            CartItems = result;
    //            CalculateSummary();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Error fetching cart: {ex.Message}");
    //        CartItems = new List<ItemsBasket>();
    //    }
    //}

    private async Task LoadCartData()
    {
        try
        {
            // حاول تجلب من السيرفر أولاً
            var result = await BasketService.GetCustomerBasketAsync(currentCustomerId);

            if (result != null && result.Any())
            {
                CartItems = result;
            }
            else
            {
                // إذا الداتا فاضية، نحط داتا تجريبية عشان نشوف الشغل
                CartItems = new List<ItemsBasket>
            {
                new ItemsBasket { ItemId = 101, QTY = 2, CustomerId = 1 },
                new ItemsBasket { ItemId = 105, QTY = 1, CustomerId = 1 },
                new ItemsBasket { ItemId = 110, QTY = 3, CustomerId = 1 }
            };
            }
            CalculateSummary();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            // حتى لو فيه ايرور، اعرض داتا وهمية عشان تشيك التصميم
            CartItems = new List<ItemsBasket>
        {
            new ItemsBasket { ItemId = 99, QTY = 1, CustomerId = 1 }
        };
            CalculateSummary();
        }
    }

    private void CalculateSummary()
    {
        if (CartItems != null)
        {
            // ملاحظة: السعر حالياً ثابت 26.00 لأن الموديل لا يحتوي على Price
            subtotal = CartItems.Sum(x => 26.00m * x.QTY);
        }
        StateHasChanged();
    }

    private async Task UpdateQuantity(ItemsBasket item, int newQty)
    {
        if (newQty < 1) return;
        item.QTY = newQty;
        var success = await BasketService.AddToBasketAsync(item);
        if (success) await LoadCartData();
    }

    private async Task RemoveItem(ItemsBasket item)
    {
        // حذف محلي للتجربة (بما أن السيرفس لا يحتوي Delete)
        CartItems?.Remove(item);
        CalculateSummary();
    }

    // إضافة الدالة التي كانت مفقودة وتسبب الخطأ
    private void ClearAll()
    {
        if (CartItems != null)
        {
            CartItems.Clear();
            CalculateSummary();
        }
    }

    private void ProceedToCheckout()
    {
        Navigation.NavigateTo("/checkout");
    }
}