using IdentityApp.Domain;
using IdentityApp.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IdentityApp.Infrastructure
{
    public class CartModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            return new CartModelBinder();
        }
    }
        public class CartModelBinder : IModelBinder
        {
            private const string sessionKey = "Cart";

            public async Task BindModelAsync(ModelBindingContext bindingContext)
            {
                // Получить объект Cart из сеанса
                Cart? cart = null;
                if (bindingContext.ActionContext.HttpContext.Session != null)
                {
                    cart = bindingContext.ActionContext.HttpContext.Session.Get<Cart>(sessionKey);
                }


                // Создать объект Cart если он не обнаружен в сеансе
                if (cart == null)
                {
                    cart = new Cart();
                    bindingContext.ActionContext.HttpContext.Session?.Set<Cart>(sessionKey, cart);
                }

                bindingContext.Model = cart;
            }
        }
    }

