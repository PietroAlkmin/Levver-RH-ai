/**
 * 🎨 Levver.ai Design System - Color Tokens
 * Paleta de cores oficial em TypeScript
 */

export const LevverColors = {
  // ===== CORES PRIMÁRIAS =====
  purple: '#A417D0',
  darkPurple: '#11005D',
  white: '#FBFBFF',
  gray: '#EAEAF0',
  lavender: '#D4C2F5',
  error: '#E84358',

  // ===== ESTADOS =====
  success: '#10B981',
  warning: '#F59E0B',
  info: '#3B82F6',

  // ===== LIGHT MODE =====
  light: {
    background: {
      primary: '#FBFBFF',
      secondary: '#EAEAF0',
      tertiary: '#FFFFFF',
    },
    text: {
      primary: '#11005D',
      secondary: '#666666',
      tertiary: '#999999',
    },
    border: {
      default: '#EAEAF0',
      hover: '#A417D0',
    },
  },

  // ===== DARK MODE =====
  dark: {
    background: {
      primary: '#11005D',
      secondary: '#1A0070',
      tertiary: '#250085',
    },
    text: {
      primary: '#FBFBFF',
      secondary: '#D4C2F5',
      tertiary: '#A892D8',
    },
    border: {
      default: '#3D2A7A',
      hover: '#A417D0',
    },
  },
} as const;

export const LevverGradients = {
  primary: 'linear-gradient(135deg, #A417D0 0%, #11005D 100%)',
  light: 'linear-gradient(135deg, #FBFBFF 0%, #D4C2F5 100%)',
  cardHover: 'linear-gradient(135deg, #D4C2F5 0%, #A417D0 100%)',
} as const;

export const LevverShadows = {
  sm: '0 1px 2px rgba(17, 0, 93, 0.05)',
  md: '0 4px 6px rgba(17, 0, 93, 0.1)',
  lg: '0 10px 15px rgba(17, 0, 93, 0.15)',
  xl: '0 20px 25px rgba(17, 0, 93, 0.2)',
  accent: '0 4px 12px rgba(164, 23, 208, 0.15)',
  accentLg: '0 10px 20px rgba(164, 23, 208, 0.25)',
} as const;

export const LevverSpacing = {
  xs: '4px',
  sm: '8px',
  md: '16px',
  lg: '24px',
  xl: '32px',
  '2xl': '48px',
  '3xl': '64px',
} as const;

export const LevverRadius = {
  sm: '4px',
  md: '8px',
  lg: '12px',
  xl: '16px',
  full: '9999px',
} as const;

export const LevverTransitions = {
  fast: '150ms ease',
  base: '300ms ease',
  slow: '500ms ease',
} as const;

// ===== TYPE EXPORTS =====
export type LevverColorKey = keyof typeof LevverColors;
export type LevverGradientKey = keyof typeof LevverGradients;
export type LevverShadowKey = keyof typeof LevverShadows;
export type LevverSpacingKey = keyof typeof LevverSpacing;
export type LevverRadiusKey = keyof typeof LevverRadius;
export type LevverTransitionKey = keyof typeof LevverTransitions;

// ===== HELPER FUNCTIONS =====

/**
 * Retorna cor com opacidade
 * @param hex Cor em hexadecimal
 * @param alpha Opacidade (0-1)
 */
export const withOpacity = (hex: string, alpha: number): string => {
  const r = parseInt(hex.slice(1, 3), 16);
  const g = parseInt(hex.slice(3, 5), 16);
  const b = parseInt(hex.slice(5, 7), 16);
  return `rgba(${r}, ${g}, ${b}, ${alpha})`;
};

/**
 * Retorna cor mais escura
 * @param hex Cor em hexadecimal
 * @param percent Porcentagem (0-100)
 */
export const darken = (hex: string, percent: number): string => {
  const num = parseInt(hex.replace('#', ''), 16);
  const amt = Math.round(2.55 * percent);
  const R = (num >> 16) - amt;
  const G = ((num >> 8) & 0x00ff) - amt;
  const B = (num & 0x0000ff) - amt;
  return `#${(
    0x1000000 +
    (R < 255 ? (R < 1 ? 0 : R) : 255) * 0x10000 +
    (G < 255 ? (G < 1 ? 0 : G) : 255) * 0x100 +
    (B < 255 ? (B < 1 ? 0 : B) : 255)
  )
    .toString(16)
    .slice(1)
    .toUpperCase()}`;
};

/**
 * Retorna cor mais clara
 * @param hex Cor em hexadecimal
 * @param percent Porcentagem (0-100)
 */
export const lighten = (hex: string, percent: number): string => {
  const num = parseInt(hex.replace('#', ''), 16);
  const amt = Math.round(2.55 * percent);
  const R = (num >> 16) + amt;
  const G = ((num >> 8) & 0x00ff) + amt;
  const B = (num & 0x0000ff) + amt;
  return `#${(
    0x1000000 +
    (R < 255 ? (R < 1 ? 0 : R) : 255) * 0x10000 +
    (G < 255 ? (G < 1 ? 0 : G) : 255) * 0x100 +
    (B < 255 ? (B < 1 ? 0 : B) : 255)
  )
    .toString(16)
    .slice(1)
    .toUpperCase()}`;
};

// ===== DEFAULT EXPORT =====
export default {
  colors: LevverColors,
  gradients: LevverGradients,
  shadows: LevverShadows,
  spacing: LevverSpacing,
  radius: LevverRadius,
  transitions: LevverTransitions,
  utils: {
    withOpacity,
    darken,
    lighten,
  },
};
