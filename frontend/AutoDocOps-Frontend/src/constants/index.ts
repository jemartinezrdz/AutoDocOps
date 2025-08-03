import Constants from 'expo-constants';

// API Configuration
export const API_CONFIG = {
  BASE_URL: __DEV__ 
    ? 'http://localhost:5001/api' 
    : 'https://autodocops-api.fly.dev/api',
  TIMEOUT: 30000,
  RETRY_ATTEMPTS: 3,
};

// Storage Keys
export const STORAGE_KEYS = {
  AUTH_TOKEN: 'auth_token',
  USER_DATA: 'user_data',
  THEME_PREFERENCE: 'theme_preference',
  LANGUAGE_PREFERENCE: 'language_preference',
  ONBOARDING_COMPLETED: 'onboarding_completed',
} as const;

// App Configuration
export const APP_CONFIG = {
  NAME: 'AutoDocOps',
  VERSION: Constants.expoConfig?.version || '1.0.0',
  BUILD_NUMBER: Constants.expoConfig?.ios?.buildNumber || '1',
  BUNDLE_ID: Constants.expoConfig?.ios?.bundleIdentifier || 'com.autodocops.app',
  PACKAGE_NAME: Constants.expoConfig?.android?.package || 'com.autodocops.app',
};

// Theme Colors
export const COLORS = {
  primary: '#2563EB',
  primaryDark: '#1D4ED8',
  primaryLight: '#3B82F6',
  secondary: '#10B981',
  secondaryDark: '#059669',
  secondaryLight: '#34D399',
  
  background: '#FFFFFF',
  backgroundDark: '#0F172A',
  surface: '#F8FAFC',
  surfaceDark: '#1E293B',
  
  text: '#0F172A',
  textDark: '#F8FAFC',
  textSecondary: '#64748B',
  textSecondaryDark: '#94A3B8',
  
  border: '#E2E8F0',
  borderDark: '#334155',
  
  error: '#EF4444',
  errorDark: '#DC2626',
  warning: '#F59E0B',
  warningDark: '#D97706',
  success: '#10B981',
  successDark: '#059669',
  info: '#3B82F6',
  infoDark: '#2563EB',
  
  gray: {
    50: '#F8FAFC',
    100: '#F1F5F9',
    200: '#E2E8F0',
    300: '#CBD5E1',
    400: '#94A3B8',
    500: '#64748B',
    600: '#475569',
    700: '#334155',
    800: '#1E293B',
    900: '#0F172A',
  },
} as const;

// Spacing
export const SPACING = {
  xs: 4,
  sm: 8,
  md: 16,
  lg: 24,
  xl: 32,
  xxl: 48,
} as const;

// Typography
export const TYPOGRAPHY = {
  h1: 32,
  h2: 28,
  h3: 24,
  h4: 20,
  h5: 18,
  h6: 16,
  body: 14,
  caption: 12,
  small: 10,
} as const;

// Border Radius
export const BORDER_RADIUS = {
  sm: 4,
  md: 8,
  lg: 12,
  xl: 16,
  full: 9999,
} as const;

// Project Types
export const PROJECT_TYPES = [
  { value: 1, label: 'API .NET', description: 'Documentar APIs desarrolladas en .NET' },
  { value: 2, label: 'Base de Datos SQL Server', description: 'Documentar esquemas de SQL Server' },
  { value: 3, label: 'Híbrido', description: 'Combinar API .NET y Base de Datos' },
] as const;

// Project Status
export const PROJECT_STATUS = [
  { value: 1, label: 'Creado', color: COLORS.gray[500] },
  { value: 2, label: 'Configurado', color: COLORS.info },
  { value: 3, label: 'Analizando', color: COLORS.warning },
  { value: 4, label: 'Analizado', color: COLORS.success },
  { value: 5, label: 'Documentación Generada', color: COLORS.primary },
  { value: 6, label: 'Error', color: COLORS.error },
  { value: 7, label: 'Pausado', color: COLORS.gray[400] },
] as const;

// Languages
export const LANGUAGES = [
  { value: 1, label: 'Español', code: 'es' },
  { value: 2, label: 'English', code: 'en' },
] as const;

// Authentication Types
export const AUTH_TYPES = [
  { value: 'Bearer', label: 'Token Bearer' },
  { value: 'ApiKey', label: 'API Key' },
  { value: 'Windows', label: 'Autenticación Windows' },
  { value: 'SqlServer', label: 'SQL Server Authentication' },
] as const;

// Diagram Formats
export const DIAGRAM_FORMATS = [
  { value: 'PNG', label: 'PNG' },
  { value: 'SVG', label: 'SVG' },
  { value: 'PDF', label: 'PDF' },
] as const;

// Themes
export const THEMES = [
  { value: 'Default', label: 'Por Defecto' },
  { value: 'Dark', label: 'Oscuro' },
  { value: 'Blue', label: 'Azul' },
  { value: 'Green', label: 'Verde' },
] as const;

// Validation Rules
export const VALIDATION = {
  EMAIL_REGEX: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
  PASSWORD_MIN_LENGTH: 8,
  PROJECT_NAME_MIN_LENGTH: 3,
  PROJECT_NAME_MAX_LENGTH: 200,
  DESCRIPTION_MAX_LENGTH: 1000,
  URL_REGEX: /^https?:\/\/.+/,
} as const;

// Error Messages
export const ERROR_MESSAGES = {
  NETWORK_ERROR: 'Error de conexión. Verifica tu conexión a internet.',
  UNAUTHORIZED: 'No tienes autorización para realizar esta acción.',
  FORBIDDEN: 'Acceso denegado.',
  NOT_FOUND: 'Recurso no encontrado.',
  SERVER_ERROR: 'Error interno del servidor. Intenta nuevamente.',
  VALIDATION_ERROR: 'Los datos proporcionados no son válidos.',
  TIMEOUT_ERROR: 'La solicitud ha tardado demasiado. Intenta nuevamente.',
  UNKNOWN_ERROR: 'Ha ocurrido un error inesperado.',
} as const;

// Success Messages
export const SUCCESS_MESSAGES = {
  PROJECT_CREATED: 'Proyecto creado exitosamente',
  PROJECT_UPDATED: 'Proyecto actualizado exitosamente',
  PROJECT_DELETED: 'Proyecto eliminado exitosamente',
  DOCUMENTATION_GENERATED: 'Documentación generada exitosamente',
  SETTINGS_SAVED: 'Configuración guardada exitosamente',
  LOGIN_SUCCESS: 'Inicio de sesión exitoso',
  LOGOUT_SUCCESS: 'Sesión cerrada exitosamente',
  REGISTER_SUCCESS: 'Cuenta creada exitosamente',
} as const;

// Animation Durations
export const ANIMATION = {
  FAST: 150,
  NORMAL: 300,
  SLOW: 500,
} as const;

// Screen Dimensions
export const SCREEN = {
  TABLET_MIN_WIDTH: 768,
  DESKTOP_MIN_WIDTH: 1024,
} as const;

// Pagination
export const PAGINATION = {
  DEFAULT_PAGE_SIZE: 20,
  MAX_PAGE_SIZE: 100,
} as const;

