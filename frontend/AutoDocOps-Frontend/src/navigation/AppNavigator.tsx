import React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createStackNavigator } from '@react-navigation/stack';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { Ionicons } from '@expo/vector-icons';
import { useAuth } from '../contexts/AuthContext';
import { RootStackParamList, MainTabParamList } from '../types';
import { COLORS } from '../constants';

// Import screens (we'll create these next)
import LoadingScreen from '../screens/LoadingScreen';
import LoginScreen from '../screens/auth/LoginScreen';
import RegisterScreen from '../screens/auth/RegisterScreen';
import ProjectsListScreen from '../screens/projects/ProjectsListScreen';
import CreateProjectScreen from '../screens/projects/CreateProjectScreen';
import ProjectDetailScreen from '../screens/projects/ProjectDetailScreen';
import DocumentationScreen from '../screens/documentation/DocumentationScreen';
import ChatScreen from '../screens/chat/ChatScreen';
import ProfileScreen from '../screens/profile/ProfileScreen';
import SettingsScreen from '../screens/settings/SettingsScreen';

const RootStack = createStackNavigator<RootStackParamList>();
const MainTab = createBottomTabNavigator<MainTabParamList>();

// Main Tab Navigator
const MainTabNavigator: React.FC = () => {
  return (
    <MainTab.Navigator
      screenOptions={({ route }) => ({
        tabBarIcon: ({ focused, color, size }) => {
          let iconName: keyof typeof Ionicons.glyphMap;

          switch (route.name) {
            case 'Projects':
              iconName = focused ? 'folder' : 'folder-outline';
              break;
            case 'Documentation':
              iconName = focused ? 'document-text' : 'document-text-outline';
              break;
            case 'Chat':
              iconName = focused ? 'chatbubbles' : 'chatbubbles-outline';
              break;
            case 'Profile':
              iconName = focused ? 'person' : 'person-outline';
              break;
            default:
              iconName = 'help-outline';
          }

          return <Ionicons name={iconName} size={size} color={color} />;
        },
        tabBarActiveTintColor: COLORS.primary,
        tabBarInactiveTintColor: COLORS.textSecondary,
        tabBarStyle: {
          backgroundColor: COLORS.background,
          borderTopColor: COLORS.border,
          paddingBottom: 5,
          paddingTop: 5,
          height: 60,
        },
        headerStyle: {
          backgroundColor: COLORS.primary,
        },
        headerTintColor: COLORS.background,
        headerTitleStyle: {
          fontWeight: 'bold',
        },
      })}
    >
      <MainTab.Screen
        name="Projects"
        component={ProjectsListScreen}
        options={{
          title: 'Proyectos',
          headerTitle: 'Mis Proyectos',
        }}
      />
      <MainTab.Screen
        name="Documentation"
        component={DocumentationScreen}
        options={{
          title: 'Documentación',
          headerTitle: 'Documentación',
        }}
      />
      <MainTab.Screen
        name="Chat"
        component={ChatScreen}
        options={{
          title: 'Chat',
          headerTitle: 'Chat Semántico',
        }}
      />
      <MainTab.Screen
        name="Profile"
        component={ProfileScreen}
        options={{
          title: 'Perfil',
          headerTitle: 'Mi Perfil',
        }}
      />
    </MainTab.Navigator>
  );
};

// Auth Stack Navigator
const AuthStackNavigator: React.FC = () => {
  return (
    <RootStack.Navigator
      screenOptions={{
        headerStyle: {
          backgroundColor: COLORS.primary,
        },
        headerTintColor: COLORS.background,
        headerTitleStyle: {
          fontWeight: 'bold',
        },
      }}
    >
      <RootStack.Screen
        name="Login"
        component={LoginScreen}
        options={{
          title: 'Iniciar Sesión',
          headerShown: false,
        }}
      />
      <RootStack.Screen
        name="Register"
        component={RegisterScreen}
        options={{
          title: 'Crear Cuenta',
          headerShown: false,
        }}
      />
    </RootStack.Navigator>
  );
};

// Main App Navigator
const AppNavigator: React.FC = () => {
  const { state } = useAuth();

  if (state.isLoading) {
    return <LoadingScreen />;
  }

  return (
    <NavigationContainer>
      <RootStack.Navigator
        screenOptions={{
          headerShown: false,
        }}
      >
        {state.isAuthenticated ? (
          <>
            <RootStack.Screen name="Main" component={MainTabNavigator} />
            <RootStack.Screen
              name="CreateProject"
              component={CreateProjectScreen}
              options={{
                headerShown: true,
                title: 'Crear Proyecto',
                headerStyle: {
                  backgroundColor: COLORS.primary,
                },
                headerTintColor: COLORS.background,
                headerTitleStyle: {
                  fontWeight: 'bold',
                },
              }}
            />
            <RootStack.Screen
              name="ProjectDetail"
              component={ProjectDetailScreen}
              options={{
                headerShown: true,
                title: 'Detalle del Proyecto',
                headerStyle: {
                  backgroundColor: COLORS.primary,
                },
                headerTintColor: COLORS.background,
                headerTitleStyle: {
                  fontWeight: 'bold',
                },
              }}
            />
            <RootStack.Screen
              name="Settings"
              component={SettingsScreen}
              options={{
                headerShown: true,
                title: 'Configuración',
                headerStyle: {
                  backgroundColor: COLORS.primary,
                },
                headerTintColor: COLORS.background,
                headerTitleStyle: {
                  fontWeight: 'bold',
                },
              }}
            />
          </>
        ) : (
          <RootStack.Screen name="Auth" component={AuthStackNavigator} />
        )}
      </RootStack.Navigator>
    </NavigationContainer>
  );
};

export default AppNavigator;

