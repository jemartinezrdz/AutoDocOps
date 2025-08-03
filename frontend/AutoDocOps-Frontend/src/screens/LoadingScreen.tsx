import React from 'react';
import { View, Text, ActivityIndicator, StyleSheet } from 'react-native';
import { LinearGradient } from 'expo-linear-gradient';
import { COLORS, TYPOGRAPHY, APP_CONFIG } from '../constants';

const LoadingScreen: React.FC = () => {
  return (
    <LinearGradient
      colors={[COLORS.primary, COLORS.primaryDark]}
      style={styles.container}
    >
      <View style={styles.content}>
        <Text style={styles.title}>{APP_CONFIG.NAME}</Text>
        <Text style={styles.subtitle}>Generador automático de documentación</Text>
        
        <View style={styles.loadingContainer}>
          <ActivityIndicator size="large" color={COLORS.background} />
          <Text style={styles.loadingText}>Cargando...</Text>
        </View>
        
        <Text style={styles.version}>v{APP_CONFIG.VERSION}</Text>
      </View>
    </LinearGradient>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
  content: {
    alignItems: 'center',
    paddingHorizontal: 32,
  },
  title: {
    fontSize: TYPOGRAPHY.h1,
    fontWeight: 'bold',
    color: COLORS.background,
    marginBottom: 8,
    textAlign: 'center',
  },
  subtitle: {
    fontSize: TYPOGRAPHY.body,
    color: COLORS.background,
    textAlign: 'center',
    opacity: 0.9,
    marginBottom: 48,
  },
  loadingContainer: {
    alignItems: 'center',
    marginBottom: 48,
  },
  loadingText: {
    fontSize: TYPOGRAPHY.body,
    color: COLORS.background,
    marginTop: 16,
    opacity: 0.8,
  },
  version: {
    fontSize: TYPOGRAPHY.caption,
    color: COLORS.background,
    opacity: 0.7,
  },
});

export default LoadingScreen;

