// Tipos principales para AutoDocOps Frontend

export interface User {
  id: string;
  email: string;
  name: string;
  avatar?: string;
  createdAt: string;
  updatedAt: string;
}

export interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
}

export enum ProjectType {
  DotNetApi = 1,
  SqlServerDatabase = 2,
  Hybrid = 3,
}

export enum ProjectStatus {
  Created = 1,
  Configured = 2,
  Analyzing = 3,
  Analyzed = 4,
  DocumentationGenerated = 5,
  Error = 6,
  Paused = 7,
}

export enum Language {
  Spanish = 1,
  English = 2,
}

export interface ConnectionConfig {
  connectionString: string;
  authenticationType: string;
  username?: string;
  accessToken?: string;
  additionalSettings?: string;
  isEnabled: boolean;
  timeoutSeconds: number;
}

export interface DocumentationConfig {
  generateOpenApi: boolean;
  generateSwaggerUI: boolean;
  generatePostmanCollection: boolean;
  generateTypeScriptSDK: boolean;
  generateCSharpSDK: boolean;
  generateERDiagrams: boolean;
  generateDataDictionary: boolean;
  generateUsageGuides: boolean;
  enableSemanticChat: boolean;
  diagramFormat: string;
  theme: string;
  customSettings?: string;
  includeCodeExamples: boolean;
  includeVersioning: boolean;
}

export interface Project {
  id: string;
  name: string;
  description: string;
  type: ProjectType;
  status: ProjectStatus;
  connectionConfig: ConnectionConfig;
  repositoryUrl?: string;
  branch?: string;
  preferredLanguage: Language;
  documentationConfig: DocumentationConfig;
  lastAnalyzedAt?: string;
  version: string;
  createdAt: string;
  updatedAt: string;
  createdBy: string;
  updatedBy: string;
  isActive: boolean;
}

export interface ApiDocumentation {
  id: string;
  projectId: string;
  apiName: string;
  version: string;
  baseUrl: string;
  description: string;
  openApiSpec: string;
  postmanCollection?: string;
  typeScriptSDK?: string;
  cSharpSDK?: string;
  usageGuides?: string;
  language: Language;
  lastGeneratedAt?: string;
  metadata?: string;
  createdAt: string;
  updatedAt: string;
}

export interface DatabaseSchema {
  id: string;
  projectId: string;
  databaseName: string;
  schemaName: string;
  version: string;
  description: string;
  schemaDefinition: string;
  erDiagram?: string;
  dataDictionary?: string;
  sampleQueries?: string;
  storedProceduresDoc?: string;
  functionsDoc?: string;
  triggersDoc?: string;
  usageGuides?: string;
  language: Language;
  lastGeneratedAt?: string;
  metadata?: string;
  tableCount: number;
  viewCount: number;
  storedProcedureCount: number;
  functionCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
}

export interface PaginatedResponse<T> {
  data: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface CreateProjectRequest {
  name: string;
  description: string;
  type: ProjectType;
  connectionConfig: ConnectionConfig;
  repositoryUrl?: string;
  branch?: string;
  preferredLanguage: Language;
  documentationConfig: DocumentationConfig;
}

export interface UpdateProjectRequest {
  name?: string;
  description?: string;
  connectionConfig?: ConnectionConfig;
  repositoryUrl?: string;
  branch?: string;
  documentationConfig?: DocumentationConfig;
}

export interface GenerateDocumentationRequest {
  projectId: string;
  forceRegenerate?: boolean;
}

export interface ChatMessage {
  id: string;
  content: string;
  role: 'user' | 'assistant';
  timestamp: string;
  projectId?: string;
}

export interface SemanticSearchRequest {
  query: string;
  projectId?: string;
  limit?: number;
}

export interface SemanticSearchResult {
  id: string;
  content: string;
  score: number;
  type: 'api' | 'database';
  metadata?: Record<string, any>;
}

// Navigation types
export type RootStackParamList = {
  Auth: undefined;
  Login: undefined;
  Register: undefined;
  Main: undefined;
  CreateProject: undefined;
  ProjectDetail: { projectId: string };
  DocumentationViewer: { projectId: string; type: 'api' | 'database' };
  Settings: undefined;
};

export type MainTabParamList = {
  Projects: undefined;
  Documentation: undefined;
  Chat: undefined;
  Profile: undefined;
};

export type ProjectsStackParamList = {
  ProjectsList: undefined;
  CreateProject: undefined;
  EditProject: { projectId: string };
  ProjectDetail: { projectId: string };
};

// Form types
export interface LoginForm {
  email: string;
  password: string;
}

export interface RegisterForm {
  name: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface ProjectForm {
  name: string;
  description: string;
  type: ProjectType;
  repositoryUrl?: string;
  branch?: string;
  preferredLanguage: Language;
  connectionString: string;
  authenticationType: string;
  username?: string;
  accessToken?: string;
  generateOpenApi: boolean;
  generateSwaggerUI: boolean;
  generatePostmanCollection: boolean;
  generateTypeScriptSDK: boolean;
  generateCSharpSDK: boolean;
  generateERDiagrams: boolean;
  generateDataDictionary: boolean;
  generateUsageGuides: boolean;
  enableSemanticChat: boolean;
}

// Error types
export interface ApiError {
  statusCode: number;
  message: string;
  details?: string;
  timestamp: string;
  path?: string;
  correlationId: string;
}

// Theme types
export interface Theme {
  colors: {
    primary: string;
    secondary: string;
    background: string;
    surface: string;
    text: string;
    textSecondary: string;
    border: string;
    error: string;
    warning: string;
    success: string;
    info: string;
  };
  spacing: {
    xs: number;
    sm: number;
    md: number;
    lg: number;
    xl: number;
  };
  typography: {
    h1: number;
    h2: number;
    h3: number;
    h4: number;
    body: number;
    caption: number;
  };
  borderRadius: {
    sm: number;
    md: number;
    lg: number;
  };
}

