namespace test.Configuration
{
    public static class SwaggerConfiguration
    {
        /// <summary>
        /// Gets the custom CSS and JavaScript for Swagger UI
        /// </summary>
        public static string GetSwaggerUICustomization()
        {
            return @"
            <style>
                .swagger-ui .topbar { display: none; }
                .swagger-ui .information-container { 
                    padding: 10px !important; 
                    margin-top: 20px !important;
                    display: flex !important;
                    justify-content: space-between !important;
                    align-items: center !important;
                }
                .swagger-ui .information-container .info { margin: 0 !important; }
                .swagger-ui .information-container .info .title { margin: 0 !important; }
                .swagger-ui .information-container .info > div:not(.title) { display: none !important; }
                .swagger-ui .information-container .info .link { display: none !important; }
                .swagger-ui .information-container .info .version { display: none !important; }
                .swagger-ui .information-container .info .title span { display: none !important; }
                .swagger-ui .scheme-container { display: none; }
                .swagger-ui .servers-container { display: none; }
                .custom-button {
                    background-color: #4CAF50 !important;
                    border: none !important;
                    color: white !important;
                    padding: 10px 20px !important;
                    text-align: center !important;
                    text-decoration: none !important;
                    display: inline-block !important;
                    font-size: 16px !important;
                    margin: 4px 2px !important;
                    cursor: pointer !important;
                    border-radius: 4px !important;
                    transition: background-color 0.3s !important;
                }
                .custom-button:hover {
                    background-color: #45a049 !important;
                }
            </style>
            <script>
                function addButton() {
                    try {
                        const container = document.querySelector('.information-container');
                        if (!container) {
                            setTimeout(addButton, 100);
                            return;
                        }
                        
                        if (!document.querySelector('.custom-button')) {
                            const button = document.createElement('button');
                            button.className = 'custom-button';
                            button.textContent = 'View JSON';
                            button.onclick = function() {
                                window.location.href = '/swagger/v1/swagger.json';
                            };
                            container.appendChild(button);
                        }
                    } catch (error) {
                        console.error('Error adding button:', error);
                    }
                }

                // Try to add button when DOM is loaded
                if (document.readyState === 'loading') {
                    document.addEventListener('DOMContentLoaded', addButton);
                } else {
                    addButton();
                }

                // Also try after window load
                window.addEventListener('load', addButton);
            </script>";
        }
    }
} 