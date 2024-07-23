using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace OptimizelySAML.Middleware
{
    public class CustomCookieOptionsMonitor : IOptionsMonitor<CookieAuthenticationOptions>
    {
        private readonly IOptionsFactory<CookieAuthenticationOptions> _optionsFactory;
        private CookieAuthenticationOptions _currentValue;

        public CustomCookieOptionsMonitor(IOptionsFactory<CookieAuthenticationOptions> optionsFactory)
        {
            _optionsFactory = optionsFactory;
        }

        public CookieAuthenticationOptions CurrentValue
        {
            get
            {
                if (_currentValue == null)
                {
                    _currentValue = _optionsFactory.Create(Options.DefaultName);
                    // Overwrite the LoginPath dynamically
                    _currentValue.LoginPath = new Microsoft.AspNetCore.Http.PathString("/en/azureloginpage/");
                }
                return _currentValue;
            }
        }

        public CookieAuthenticationOptions Get(string name)
        {
            var options = _optionsFactory.Create(name);
            // Overwrite the LoginPath for specific named options if needed
            options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/en/azureloginpage/");
            return options;
        }

        public IDisposable OnChange(Action<CookieAuthenticationOptions, string> listener) => null;
    }
}
