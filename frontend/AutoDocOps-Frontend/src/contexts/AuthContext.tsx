import React, { createContext, useContext, useReducer, useEffect, ReactNode } from 'react';
import * as SecureStore from 'expo-secure-store';
import { AuthState, User, LoginForm, RegisterForm } from '../types';
import { STORAGE_KEYS } from '../constants';
import apiService from '../services/api';

// Action types
type AuthAction =
  | { type: 'SET_LOADING'; payload: boolean }
  | { type: 'LOGIN_SUCCESS'; payload: { user: User; token: string } }
  | { type: 'LOGOUT' }
  | { type: 'UPDATE_USER'; payload: User }
  | { type: 'RESTORE_SESSION'; payload: { user: User; token: string } };

// Initial state
const initialState: AuthState = {
  user: null,
  token: null,
  isAuthenticated: false,
  isLoading: true,
};

// Reducer
const authReducer = (state: AuthState, action: AuthAction): AuthState => {
  switch (action.type) {
    case 'SET_LOADING':
      return {
        ...state,
        isLoading: action.payload,
      };
    case 'LOGIN_SUCCESS':
      return {
        ...state,
        user: action.payload.user,
        token: action.payload.token,
        isAuthenticated: true,
        isLoading: false,
      };
    case 'LOGOUT':
      return {
        ...state,
        user: null,
        token: null,
        isAuthenticated: false,
        isLoading: false,
      };
    case 'UPDATE_USER':
      return {
        ...state,
        user: action.payload,
      };
    case 'RESTORE_SESSION':
      return {
        ...state,
        user: action.payload.user,
        token: action.payload.token,
        isAuthenticated: true,
        isLoading: false,
      };
    default:
      return state;
  }
};

// Context type
interface AuthContextType {
  state: AuthState;
  login: (credentials: LoginForm) => Promise<void>;
  register: (userData: RegisterForm) => Promise<void>;
  logout: () => Promise<void>;
  updateUser: (user: User) => void;
  refreshToken: () => Promise<void>;
}

// Create context
const AuthContext = createContext<AuthContextType | undefined>(undefined);

// Provider props
interface AuthProviderProps {
  children: ReactNode;
}

// Provider component
export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [state, dispatch] = useReducer(authReducer, initialState);

  // Restore session on app start
  useEffect(() => {
    restoreSession();
  }, []);

  const restoreSession = async (): Promise<void> => {
    try {
      dispatch({ type: 'SET_LOADING', payload: true });

      const token = await SecureStore.getItemAsync(STORAGE_KEYS.AUTH_TOKEN);
      const userData = await SecureStore.getItemAsync(STORAGE_KEYS.USER_DATA);

      if (token && userData) {
        const user = JSON.parse(userData);
        await apiService.setAuthToken(token);
        
        // Verify token is still valid
        try {
          const response = await apiService.get('/auth/verify');
          if (response.success) {
            dispatch({ type: 'RESTORE_SESSION', payload: { user, token } });
          } else {
            await clearStoredSession();
            dispatch({ type: 'SET_LOADING', payload: false });
          }
        } catch (error) {
          await clearStoredSession();
          dispatch({ type: 'SET_LOADING', payload: false });
        }
      } else {
        dispatch({ type: 'SET_LOADING', payload: false });
      }
    } catch (error) {
      // Error restoring session - user will need to login again
      dispatch({ type: 'SET_LOADING', payload: false });
    }
  };

  const login = async (credentials: LoginForm): Promise<void> => {
    try {
      dispatch({ type: 'SET_LOADING', payload: true });

      const response = await apiService.post('/auth/login', credentials);
      
      if (response.success && response.data) {
        const { user, token } = response.data;
        
        // Store credentials securely
        await SecureStore.setItemAsync(STORAGE_KEYS.AUTH_TOKEN, token);
        await SecureStore.setItemAsync(STORAGE_KEYS.USER_DATA, JSON.stringify(user));
        
        // Set token in API service
        await apiService.setAuthToken(token);
        
        dispatch({ type: 'LOGIN_SUCCESS', payload: { user, token } });
      } else {
        throw new Error(response.message || 'Login failed');
      }
    } catch (error) {
      dispatch({ type: 'SET_LOADING', payload: false });
      throw error;
    }
  };

  const register = async (userData: RegisterForm): Promise<void> => {
    try {
      dispatch({ type: 'SET_LOADING', payload: true });

      const response = await apiService.post('/auth/register', userData);
      
      if (response.success && response.data) {
        const { user, token } = response.data;
        
        // Store credentials securely
        await SecureStore.setItemAsync(STORAGE_KEYS.AUTH_TOKEN, token);
        await SecureStore.setItemAsync(STORAGE_KEYS.USER_DATA, JSON.stringify(user));
        
        // Set token in API service
        await apiService.setAuthToken(token);
        
        dispatch({ type: 'LOGIN_SUCCESS', payload: { user, token } });
      } else {
        throw new Error(response.message || 'Registration failed');
      }
    } catch (error) {
      dispatch({ type: 'SET_LOADING', payload: false });
      throw error;
    }
  };

  const logout = async (): Promise<void> => {
    try {
      // Call logout endpoint
      await apiService.post('/auth/logout');
    } catch (error) {
      // Error calling logout endpoint - will clear session anyway
    } finally {
      await clearStoredSession();
      dispatch({ type: 'LOGOUT' });
    }
  };

  const updateUser = (user: User): void => {
    dispatch({ type: 'UPDATE_USER', payload: user });
    // Update stored user data
    SecureStore.setItemAsync(STORAGE_KEYS.USER_DATA, JSON.stringify(user));
  };

  const refreshToken = async (): Promise<void> => {
    try {
      const response = await apiService.post('/auth/refresh');
      
      if (response.success && response.data) {
        const { token } = response.data;
        await apiService.setAuthToken(token);
        await SecureStore.setItemAsync(STORAGE_KEYS.AUTH_TOKEN, token);
      }
    } catch (error) {
      // Error refreshing token - logout user
      await logout();
    }
  };

  const clearStoredSession = async (): Promise<void> => {
    try {
      await SecureStore.deleteItemAsync(STORAGE_KEYS.AUTH_TOKEN);
      await SecureStore.deleteItemAsync(STORAGE_KEYS.USER_DATA);
      await apiService.clearAuthToken();
    } catch (error) {
      // Error clearing stored session - operation failed silently
    }
  };

  const contextValue: AuthContextType = {
    state,
    login,
    register,
    logout,
    updateUser,
    refreshToken,
  };

  return (
    <AuthContext.Provider value={contextValue}>
      {children}
    </AuthContext.Provider>
  );
};

// Hook to use auth context
export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

export default AuthContext;

