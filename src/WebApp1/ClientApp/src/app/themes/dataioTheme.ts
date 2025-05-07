import { definePreset } from '@primeng/themes';
import Aura from '@primeng/themes/aura';

// export const DataioColors = definePreset(Aura, {
//   semantic: {
//     primary: {
//       50: '#E6EFFA',
//       100: '#C3DAF3',
//       200: '#99C3EC',
//       300: '#66A8E2',
//       400: '#3389D6',
//       500: '#054BAA', // Cobalt (primary base)
//       600: '#043C89',
//       700: '#032C67',
//       800: '#021D44',
//       900: '#010F22',
//       950: '#000811'
//     },
//     secondary: {
//       500: '#FEB035', // Supernova
//     },
//     tertiary: {
//       500: '#AA1205', // Brick
//     },
//     accent: {
//       500: '#9EAA05', // Citrus
//     },
//     neutral: {
//       500: '#3D3D3D', // Iron
//       100: '#F2F2F2'  // Light gray
//     },
//     colorScheme: {
//       light: {
//         primary: {
//           color: '#054BAA',
//           inverseColor: '#ffffff',
//           hoverColor: '#3389D6',
//           activeColor: '#043C89'
//         },
//         highlight: {
//           background: '#054BAA',
//           focusBackground: '#3389D6',
//           color: '#ffffff',
//           focusColor: '#ffffff'
//         }
//       },
//       dark: {
//         primary: {
//           color: '#C3DAF3',
//           inverseColor: '#054BAA',
//           hoverColor: '#E6EFFA',
//           activeColor: '#99C3EC'
//         },
//         highlight: {
//           background: 'rgba(255, 255, 255, 0.16)',
//           focusBackground: 'rgba(255, 255, 255, 0.24)',
//           color: 'rgba(255, 255, 255, 0.87)',
//           focusColor: 'rgba(255, 255, 255, 0.87)'
//         },
//         surface: {
//           0: '#ffffff',
//           50: '{amber.50}',
//           100: '{slate.100}',
//           200: '{slate.200}',
//           300: '{slate.300}',
//           400: '{slate.400}',
//           500: '{slate.500}',
//           600: '{slate.600}',
//           700: '{slate.700}',
//           800: '{slate.800}',
//           900: '{slate.900}',
//           950: '{slate.950}'
//         }
//       }
//     }
//   }
// });

export const DataioColors = definePreset(Aura, {
  semantic: {
    primary: {
      50: '#E6EFFA',
      100: '#C3DAF3',
      200: '#99C3EC',
      300: '#66A8E2',
      400: '#3389D6',
      500: '#054BAA', // Cobalt (primary base)
      600: '#043C89',
      700: '#032C67',
      800: '#021D44',
      900: '#010F22',
      950: '#000811'
    },
    secondary: {
      500: '#FEB035', // Supernova
    },
    tertiary: {
      500: '#AA1205', // Brick
    },
    accent: {
      500: '#9EAA05', // Citrus
    },
    neutral: {
      500: '#3D3D3D', // Iron
      100: '#F2F2F2'  // Light gray
    },
    colorScheme: {
      light: {
        primary: {
          color: '#054BAA',
          inverseColor: '#ffffff',
          hoverColor: '#3389D6',
          activeColor: '#043C89'
        },
        highlight: {
          background: 'rgba(177, 157, 247, 0.16)',
          focusBackground: 'rgba(177, 157, 247, 0.24)',
          color: '#b19df7',
          focusColor: '#b19df7'
        }
      },
      dark: {
        primary: {
          color: '#C3DAF3',
          inverseColor: '#054BAA',
          hoverColor: '#E6EFFA',
          activeColor: '#99C3EC'
        },
        highlight: {
          background: 'rgba(255, 255, 255, 0.16)',
          focusBackground: 'rgba(255, 255, 255, 0.24)',
          color: 'rgba(255, 255, 255, 0.87)',
          focusColor: 'rgba(255, 255, 255, 0.87)'
        },
        surface: {
          0:   '#ffffff',
          50:  '#e8e9e9',
          100: '#d2d2d4',
          200: '#bbbcbe',
          300: '#a5a5a9',
          400: '#8e8f93',
          500: '#77787d',
          600: '#616268',
          700: '#4a4b52',
          800: '#34343d',
          900: '#1d1e27',
          950: '#1d1e27'
        }
      }
    }
  }
});

