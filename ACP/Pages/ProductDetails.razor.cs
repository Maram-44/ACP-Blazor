//using ACP.Application.DTOs.Products;
//using ACP.Models;
//using ACP.Models.Products;
//using ACP.Services;
//using Microsoft.AspNetCore.Components;
//using Microsoft.AspNetCore.Components.Authorization;
//using System.Security.Claims;

//namespace ACP.Pages
//{
//    public partial class ProductDetails
//    {
//        [Parameter] public int ItemId { get; set; }

//        [Inject] public ProductService? ProductService { get; set; }
//        [Inject] public AuthenticationStateProvider? AuthStateProvider { get; set; }

//        public Item? item;
//        public string MainImageUrl = "";
//        public int? OpenMenuId;
//        public int CurrentCustomerId;

//        public decimal OriginalPrice => (item?.SellPrice ?? 0) / (1 - (decimal)(item?.DiscountRate ?? 0));
//        public decimal AmountSaved => OriginalPrice - (item?.SellPrice ?? 0);

//        public List<StarStat> StarStats = new();
//        public bool IsModalOpen = false;
//        public int NewRating = 0;
//        public string NewCommentText = "";

//        protected override async Task OnInitializedAsync()
//        {
//            // 1. عرض بيانات تجريبية فوراً لملء التصميم
//            LoadMockData();

//            // 2. جلب البيانات الحقيقية (بما فيها الفئة عبر العلاقة)
//            await LoadData();

//            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
//            var user = authState.User;

//            if (user.Identity?.IsAuthenticated == true)
//            {
//                var idClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("sub") ?? user.FindFirst("id");
//                if (idClaim != null) int.TryParse(idClaim.Value, out CurrentCustomerId);
//            }
//        }

//        private void LoadMockData()
//        {
//            // إنشاء كائن مؤقت بنفس هيكلية جداولك
//            item = new Item
//            {
//                ItemName = "DEODORIZNG SPRAY",
//                Description = "This is spray very good for all animals it used to prevent the teast and give it nice semall",
//                SKU = "PU8-78-OI9",
//                SellPrice = 3000,
//                DiscountRate = 30,
//                AverageRating = 5.6,
//                TotalReviews = 30,
//                // بيانات تجريبية للفئة المرتبطة
//                Category = new ItemsCategory { CategoryName = "SKIN CEARE" },
//                itemPhoto = new List<ItemPhoto>
//        {
//            new ItemPhoto { PhotoName = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=800", IsMainPhoto = true },
//            new ItemPhoto { PhotoName = "https://images.unsplash.com/photo-1484704849700-f032a568e944?w=800", IsMainPhoto = false },
//            new ItemPhoto { PhotoName = "https://images.unsplash.com/photo-1524678606370-a47ad25cb82a?w=800", IsMainPhoto = false }
//        }
//        ,
//                ProductComments = new List<ProductComment>
//        {
//            new ProductComment
//            {
//                Id = -1,
//                Comment = "Absolutely amazing sound quality! Best purchase this year.",
//                Rating = 5,
//                CreatedAt = DateTime.Now.AddDays(-3),
//                // الحل: نستخدم كائن Customer حقيقي بدلاً من new مجهول
//                Customer = new Customer { FirstName = "John", LastName = "Doe" }
//            },
//            new ProductComment
//            {
//                Id = -2,
//                Comment = "Great design and very comfortable.",
//                Rating = 4,
//                CreatedAt = DateTime.Now.AddDays(-10),
//                Customer = new Customer { FirstName = "Alice", LastName = "Smith" }
//            }
//        }
//            };
//            MainImageUrl = item.itemPhoto.First().PhotoName;
//            CalculateStarStats();
//        }

//        private async Task LoadData()
//        {
//            // جلب المنتج (تأكد أن السيرفر يستخدم Include لجدول Category)
//            var fetchedItem = await ProductService.GetProductByIdAsync(ItemId);
//            if (fetchedItem != null)
//            {
//                item = fetchedItem;
//                MainImageUrl = item.itemPhoto?.FirstOrDefault(p => p.IsMainPhoto)?.PhotoName
//                               ?? item.itemPhoto?.FirstOrDefault()?.PhotoName ?? "";
//                CalculateStarStats();
//                StateHasChanged(); // تحديث الواجهة فور وصول البيانات الحقيقية
//            }
//        }

//        private void CalculateStarStats()
//        {
//            StarStats.Clear();
//            if (item?.ProductComments == null) return;
//            for (int i = 5; i >= 1; i--)
//            {
//                int count = item.ProductComments.Count(c => c.Rating == i);
//                int total = item.ProductComments.Count;
//                int percent = total > 0 ? (count * 100 / total) : 0;
//                StarStats.Add(new StarStat { StarLevel = i, Percentage = percent });
//            }
//        }

//        public string GetFullName(dynamic? customer) => customer == null ? "مستخدم غير معروف" : $"{customer.FirstName} {customer.LastName}".Trim();

//        public string GetInitials(dynamic? customer)
//        {
//            if (customer == null) return "??";
//            string f = !string.IsNullOrEmpty(customer.FirstName) ? customer.FirstName[0].ToString() : "";
//            string l = !string.IsNullOrEmpty(customer.LastName) ? customer.LastName[0].ToString() : "";
//            return (f + l).ToUpper();
//        }

//        public async Task HandleSubmit()
//        {
//            if (string.IsNullOrEmpty(NewCommentText) || NewRating == 0) return;
//            var comment = new ProductComment { ItemId = ItemId, CustomerId = CurrentCustomerId, Comment = NewCommentText, Rating = NewRating, CreatedAt = DateTime.Now };
//            var result = await ProductService.AddCommentAsync(comment);
//            if (result != null) { await LoadData(); CloseModal(); }
//        }

//        public async Task HandleDelete(int commentId)
//        {
//            var success = await ProductService.DeleteCommentAsync(commentId, CurrentCustomerId);
//            if (success) { await LoadData(); OpenMenuId = null; }
//        }

//        public void ToggleMenu(int id) => OpenMenuId = (OpenMenuId == id) ? null : id;
//        public void SetMainImage(string url) => MainImageUrl = url;
//        public void OpenModal() => IsModalOpen = true;
//        public void CloseModal() { IsModalOpen = false; NewRating = 0; NewCommentText = ""; }

//        public void NextImage()
//        {
//            var photos = item?.itemPhoto?.ToList();
//            if (photos == null || !photos.Any()) return;
//            var currentIdx = photos.FindIndex(p => p.PhotoName == MainImageUrl);
//            MainImageUrl = photos[(currentIdx + 1) % photos.Count].PhotoName;
//        }

//        public void PrevImage()
//        {
//            var photos = item?.itemPhoto?.ToList();
//            if (photos == null || !photos.Any()) return;
//            var currentIdx = photos.FindIndex(p => p.PhotoName == MainImageUrl);
//            MainImageUrl = photos[(currentIdx - 1 + photos.Count) % photos.Count].PhotoName;
//        }

//        public class StarStat { public int StarLevel { get; set; } public int Percentage { get; set; } }
//    }
//}