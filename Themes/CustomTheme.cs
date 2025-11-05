using MudBlazor;

namespace JobApplicationTrackerV2.Themes
{
    public static class CustomTheme
    {
        public static MudTheme DefaultTheme { get; } = new MudTheme
        {
            PaletteLight = new PaletteLight
            {
                // Primära färger (huvudfärg för knappar, länkar, etc.)
                Primary = "#1976D2",     // Blå
                Secondary = "#424242",      // Mörkgrå
                Tertiary = "#FF9800",       // Orange
         
                // Bakgrundsfärger
                Background = "#8cbdb0",      // Vit bakgrund
                BackgroundGray = "#F5F5F5",  // Ljusgrå bakgrund
                Surface = "#FFFFFF",         // Ytor (kort, dialoger)
                DrawerBackground = "#FFFFFF", // Drawer bakgrund
                AppbarBackground = "#1976D2", // AppBar bakgrund
          
                // Text färger
                TextPrimary = "rgba(0,0,0,0.87)",
                TextSecondary = "rgba(0,0,0,0.54)",
                TextDisabled = "rgba(0,0,0,0.38)",
            
                // Åtgärdsfärger
                Success = "#4CAF50",        // Grön för framgång
                Info = "#2196F3",   // Blå för information
                Warning = "#FF9800",    // Orange för varning
                Error = "#F44336",    // Röd för fel
              
                // Dividers och borders
                Divider = "rgba(0,0,0,0.12)",
                DividerLight = "rgba(0,0,0,0.06)",
            
                // Hover och states
                HoverOpacity = 0.04,
                RippleOpacity = 0.1,
 },
            
             PaletteDark = new PaletteDark
       {
       // Primära färger för mörkt tema
                Primary = "#90CAF9",
                Secondary = "#CE93D8",
                Tertiary = "#FFB74D",
      
                // Bakgrundsfärger
                Background = "#1A1A1A",
                BackgroundGray = "#2A2A2A",
                Surface = "#2C2C2C",
                DrawerBackground = "#2C2C2C",
                AppbarBackground = "#1976D2",
        
                // Text färger
                TextPrimary = "rgba(255,255,255,0.87)",
                TextSecondary = "rgba(255,255,255,0.60)",
                TextDisabled = "rgba(255,255,255,0.38)",
     
                // Åtgärdsfärger
                Success = "#66BB6A",
                Info = "#42A5F5",
                Warning = "#FFA726",
                Error = "#EF5350",
         
                // Dividers och borders
                Divider = "rgba(255,255,255,0.12)",
                DividerLight = "rgba(255,255,255,0.06)",
 
                // Hover och states
                HoverOpacity = 0.08,
                RippleOpacity = 0.2,
            },
   
            Typography = new Typography
            {
              
            },
   
            LayoutProperties = new LayoutProperties
            {
                DefaultBorderRadius = "4px",
                DrawerWidthLeft = "240px",
                DrawerWidthRight = "240px",
                AppbarHeight = "64px"
            },
   
            ZIndex = new ZIndex
            {
                Drawer = 1200,
                AppBar = 1100,
                Dialog = 1300,
                Popover = 1400,
                Snackbar = 1500,
                Tooltip = 1600
            }
        };
    }
}
