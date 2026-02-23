using Microsoft.AspNetCore.Components;
using ACP.Models.Products;
using ACP.Services;
using System.Collections.Generic;

namespace ACP.Pages;

public class AddProductBase : ComponentBase
{
    [Inject] public NavigationManager Navigation { get; set; } = default!;
    [Inject] public ProductService CategoryService { get; set; } = default!; // سيرفس التصنيفات
    [Inject] public ProductService ProductService { get; set; } = default!; // سيرفس المنتجات
    [Inject] public AccountService AuthService { get; set; } = default!; // سيرفس معلومات المستخدم

    protected Item NewItem { get; set; } = new Item
    {
        itemPhoto = new List<ItemPhoto>(),
        QTY = 1,
        IsReturendable = true
    };

    protected List<ItemsCategory> Categories { get; set; } = new(); // قائمة التصنيفات من الداتابيس

    protected override async Task OnInitializedAsync()
    {
        // 1. جلب التصنيفات من قاعدة البيانات
        //Categories = await CategoryService.GetCategoriesAsync();

        // 2. تعيين الـ OwnerId تلقائياً من المستخدم المسجل دخول
        //var currentUser = await AuthService.GetCurrentUserAsync();
        //NewItem.OwnerId = currentUser?.Id;

        // ملاحظة: الـ ItemId لا نلمسه هنا، الداتابيس (Identity Specification) 
        // هي اللي بتزيده تلقائياً (Auto-increment) عند الحفظ.
    }

    // وظيفة إضافة رابط صورة جديد للقائمة
    protected void AddPhotoPlaceholder()
    {
        NewItem.itemPhoto.Add(new ItemPhoto { PhotoName = "" });
    }

    //protected async Task HandleSubmit()
    //{
    //    try
    //    {
    //        await ProductService.AddItemAsync(NewItem);
    //        Navigation.NavigateTo("/browse-products");
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Error: {ex.Message}");
    //    }
    //}
}