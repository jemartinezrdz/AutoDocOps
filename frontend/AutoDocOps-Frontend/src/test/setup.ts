import '@testing-library/jest-native/extend-expect';
import { cleanup } from '@testing-library/react-native';
import { afterEach, beforeEach, vi } from 'vitest';

// Cleanup after each test
afterEach(() => {
  cleanup();
});

// Mock React Native modules
beforeEach(() => {
  // Mock Expo modules
  vi.mock('expo-constants', () => ({
    default: {
      manifest: {},
      expoConfig: {},
    },
  }));

  vi.mock('expo-secure-store', () => ({
    getItemAsync: vi.fn(),
    setItemAsync: vi.fn(),
    deleteItemAsync: vi.fn(),
  }));

  // Mock React Navigation
  vi.mock('@react-navigation/native', () => ({
    useNavigation: () => ({
      navigate: vi.fn(),
      goBack: vi.fn(),
      canGoBack: vi.fn(() => true),
    }),
    useRoute: () => ({
      params: {},
    }),
    useFocusEffect: vi.fn(),
  }));

  // Mock React Native modules
  vi.mock('react-native', async () => {
    const RN = await vi.importActual('react-native');
    return {
      ...RN,
      Alert: {
        alert: vi.fn(),
      },
      Platform: {
        OS: 'ios',
        select: vi.fn((options: Record<string, any>) => options.ios),
      },
    };
  });
});

// Global test utilities
global.testUtils = {
  createMockNavigation: () => ({
    navigate: vi.fn(),
    goBack: vi.fn(),
    canGoBack: vi.fn(() => true),
    dispatch: vi.fn(),
    setOptions: vi.fn(),
    setParams: vi.fn(),
    addListener: vi.fn(),
    removeListener: vi.fn(),
    isFocused: vi.fn(() => true),
  }),
};