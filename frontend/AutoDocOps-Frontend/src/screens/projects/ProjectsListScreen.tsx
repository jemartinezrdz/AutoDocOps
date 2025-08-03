import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  FlatList,
  TouchableOpacity,
  StyleSheet,
  RefreshControl,
  Alert,
  ActivityIndicator,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { Project, ProjectStatus, ProjectType } from '../../types';
import { COLORS, TYPOGRAPHY, SPACING, BORDER_RADIUS, PROJECT_STATUS } from '../../constants';
import projectService from '../../services/projectService';

interface ProjectsListScreenProps {
  navigation: any;
}

const ProjectsListScreen: React.FC<ProjectsListScreenProps> = ({ navigation }) => {
  const [projects, setProjects] = useState<Project[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [refreshing, setRefreshing] = useState(false);

  useEffect(() => {
    loadProjects();
  }, []);

  const loadProjects = async (showLoading = true): Promise<void> => {
    try {
      if (showLoading) {
        setIsLoading(true);
      }
      const data = await projectService.getProjects();
      setProjects(data);
    } catch (error) {
      Alert.alert('Error', 'No se pudieron cargar los proyectos');
    } finally {
      if (showLoading) {
        setIsLoading(false);
      }
    }
  };

  const onRefresh = async (): Promise<void> => {
    setRefreshing(true);
    await loadProjects(false);
    setRefreshing(false);
  };

  const getStatusInfo = (status: ProjectStatus) => {
    return PROJECT_STATUS.find(s => s.value === status) || PROJECT_STATUS[0];
  };

  const getProjectTypeIcon = (type: ProjectType): keyof typeof Ionicons.glyphMap => {
    switch (type) {
      case ProjectType.DotNetApi:
        return 'code-outline';
      case ProjectType.SqlServerDatabase:
        return 'server-outline';
      case ProjectType.Hybrid:
        return 'layers-outline';
      default:
        return 'folder-outline';
    }
  };

  const renderProjectItem = ({ item }: { item: Project }) => {
    const statusInfo = getStatusInfo(item.status);
    
    return (
      <TouchableOpacity
        style={styles.projectCard}
        onPress={() => navigation.navigate('ProjectDetail', { projectId: item.id })}
      >
        <View style={styles.projectHeader}>
          <View style={styles.projectTitleContainer}>
            <Ionicons
              name={getProjectTypeIcon(item.type)}
              size={24}
              color={COLORS.primary}
              style={styles.projectIcon}
            />
            <View style={styles.projectInfo}>
              <Text style={styles.projectName}>{item.name}</Text>
              <Text style={styles.projectDescription} numberOfLines={2}>
                {item.description}
              </Text>
            </View>
          </View>
          <View style={[styles.statusBadge, { backgroundColor: statusInfo.color }]}>
            <Text style={styles.statusText}>{statusInfo.label}</Text>
          </View>
        </View>
        
        <View style={styles.projectFooter}>
          <Text style={styles.projectVersion}>v{item.version}</Text>
          <Text style={styles.projectDate}>
            {new Date(item.updatedAt).toLocaleDateString()}
          </Text>
        </View>
      </TouchableOpacity>
    );
  };

  const renderEmptyState = () => (
    <View style={styles.emptyState}>
      <Ionicons name="folder-open-outline" size={64} color={COLORS.textSecondary} />
      <Text style={styles.emptyStateTitle}>No hay proyectos</Text>
      <Text style={styles.emptyStateDescription}>
        Crea tu primer proyecto para comenzar a generar documentación automática
      </Text>
      <TouchableOpacity
        style={styles.createButton}
        onPress={() => navigation.navigate('CreateProject')}
      >
        <Text style={styles.createButtonText}>Crear Proyecto</Text>
      </TouchableOpacity>
    </View>
  );

  if (isLoading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color={COLORS.primary} />
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <FlatList
        data={projects}
        renderItem={renderProjectItem}
        keyExtractor={(item) => item.id}
        contentContainerStyle={[
          styles.listContainer,
          projects.length === 0 && styles.emptyContainer,
        ]}
        refreshControl={
          <RefreshControl
            refreshing={refreshing}
            onRefresh={onRefresh}
            colors={[COLORS.primary]}
          />
        }
        ListEmptyComponent={renderEmptyState}
        showsVerticalScrollIndicator={false}
      />

      {projects.length > 0 && (
        <TouchableOpacity
          style={styles.fab}
          onPress={() => navigation.navigate('CreateProject')}
        >
          <Ionicons name="add" size={24} color={COLORS.background} />
        </TouchableOpacity>
      )}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: COLORS.background,
  },
  loadingContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: COLORS.background,
  },
  listContainer: {
    padding: SPACING.md,
  },
  emptyContainer: {
    flexGrow: 1,
    justifyContent: 'center',
  },
  projectCard: {
    backgroundColor: COLORS.surface,
    borderRadius: BORDER_RADIUS.lg,
    padding: SPACING.md,
    marginBottom: SPACING.md,
    borderWidth: 1,
    borderColor: COLORS.border,
  },
  projectHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    marginBottom: SPACING.sm,
  },
  projectTitleContainer: {
    flexDirection: 'row',
    flex: 1,
    marginRight: SPACING.sm,
  },
  projectIcon: {
    marginRight: SPACING.sm,
    marginTop: 2,
  },
  projectInfo: {
    flex: 1,
  },
  projectName: {
    fontSize: TYPOGRAPHY.h4,
    fontWeight: 'bold',
    color: COLORS.text,
    marginBottom: SPACING.xs,
  },
  projectDescription: {
    fontSize: TYPOGRAPHY.body,
    color: COLORS.textSecondary,
    lineHeight: 20,
  },
  statusBadge: {
    paddingHorizontal: SPACING.sm,
    paddingVertical: SPACING.xs,
    borderRadius: BORDER_RADIUS.sm,
  },
  statusText: {
    fontSize: TYPOGRAPHY.caption,
    fontWeight: '600',
    color: COLORS.background,
  },
  projectFooter: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginTop: SPACING.sm,
    paddingTop: SPACING.sm,
    borderTopWidth: 1,
    borderTopColor: COLORS.border,
  },
  projectVersion: {
    fontSize: TYPOGRAPHY.caption,
    fontWeight: '600',
    color: COLORS.primary,
  },
  projectDate: {
    fontSize: TYPOGRAPHY.caption,
    color: COLORS.textSecondary,
  },
  emptyState: {
    alignItems: 'center',
    paddingHorizontal: SPACING.xl,
  },
  emptyStateTitle: {
    fontSize: TYPOGRAPHY.h3,
    fontWeight: 'bold',
    color: COLORS.text,
    marginTop: SPACING.lg,
    marginBottom: SPACING.sm,
  },
  emptyStateDescription: {
    fontSize: TYPOGRAPHY.body,
    color: COLORS.textSecondary,
    textAlign: 'center',
    lineHeight: 22,
    marginBottom: SPACING.xl,
  },
  createButton: {
    backgroundColor: COLORS.primary,
    paddingHorizontal: SPACING.xl,
    paddingVertical: SPACING.md,
    borderRadius: BORDER_RADIUS.md,
  },
  createButtonText: {
    fontSize: TYPOGRAPHY.body,
    fontWeight: 'bold',
    color: COLORS.background,
  },
  fab: {
    position: 'absolute',
    bottom: SPACING.lg,
    right: SPACING.lg,
    width: 56,
    height: 56,
    borderRadius: 28,
    backgroundColor: COLORS.primary,
    justifyContent: 'center',
    alignItems: 'center',
    elevation: 8,
    shadowColor: COLORS.text,
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.3,
    shadowRadius: 8,
  },
});

export default ProjectsListScreen;

