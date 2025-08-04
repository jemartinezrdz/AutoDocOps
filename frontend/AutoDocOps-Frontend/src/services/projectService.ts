import apiService from './api';
import { Project } from '../types';

const projectService = {
  async getProjects(): Promise<Project[]> {
    const response = await apiService.get<Project[]>('/projects');
    if (!response.success) {
      throw new Error(response.message || 'Failed to load projects');
    }
    return response.data;
  },
};

export default projectService;
