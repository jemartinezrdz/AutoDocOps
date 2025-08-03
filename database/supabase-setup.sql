-- AutoDocOps Database Setup for Supabase
-- Este script configura la base de datos Supabase con pgvector y las tablas necesarias

-- Habilitar extensiones necesarias
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "vector";

-- Crear esquema para AutoDocOps
CREATE SCHEMA IF NOT EXISTS autodocops;

-- Configurar Row Level Security (RLS)
ALTER DATABASE postgres SET "app.jwt_secret" TO 'your-jwt-secret-here';

-- Crear tabla de usuarios (extiende auth.users de Supabase)
CREATE TABLE IF NOT EXISTS autodocops.user_profiles (
    id UUID REFERENCES auth.users(id) ON DELETE CASCADE PRIMARY KEY,
    email TEXT UNIQUE NOT NULL,
    name TEXT NOT NULL,
    avatar_url TEXT,
    created_at TIMESTAMPTZ DEFAULT NOW() NOT NULL,
    updated_at TIMESTAMPTZ DEFAULT NOW() NOT NULL
);

-- Habilitar RLS en user_profiles
ALTER TABLE autodocops.user_profiles ENABLE ROW LEVEL SECURITY;

-- Política para que los usuarios solo puedan ver/editar su propio perfil
CREATE POLICY "Users can view own profile" ON autodocops.user_profiles
    FOR SELECT USING (auth.uid() = id);

CREATE POLICY "Users can update own profile" ON autodocops.user_profiles
    FOR UPDATE USING (auth.uid() = id);

-- Crear tabla de proyectos
CREATE TABLE IF NOT EXISTS autodocops.projects (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    description VARCHAR(1000) NOT NULL,
    type INTEGER NOT NULL, -- 1: DotNetApi, 2: SqlServerDatabase, 3: Hybrid
    status INTEGER NOT NULL DEFAULT 1, -- 1: Created, 2: Configured, etc.
    
    -- Connection Configuration
    connection_string VARCHAR(1000) NOT NULL,
    authentication_type VARCHAR(50) NOT NULL,
    username VARCHAR(100),
    access_token VARCHAR(2000),
    additional_settings VARCHAR(4000),
    connection_enabled BOOLEAN NOT NULL DEFAULT true,
    connection_timeout_seconds INTEGER NOT NULL DEFAULT 30,
    
    -- Repository Configuration
    repository_url VARCHAR(500),
    branch VARCHAR(100),
    preferred_language INTEGER NOT NULL DEFAULT 1, -- 1: Spanish, 2: English
    
    -- Documentation Configuration
    generate_open_api BOOLEAN NOT NULL DEFAULT true,
    generate_swagger_ui BOOLEAN NOT NULL DEFAULT true,
    generate_postman_collection BOOLEAN NOT NULL DEFAULT false,
    generate_typescript_sdk BOOLEAN NOT NULL DEFAULT false,
    generate_csharp_sdk BOOLEAN NOT NULL DEFAULT false,
    generate_er_diagrams BOOLEAN NOT NULL DEFAULT false,
    generate_data_dictionary BOOLEAN NOT NULL DEFAULT false,
    generate_usage_guides BOOLEAN NOT NULL DEFAULT true,
    enable_semantic_chat BOOLEAN NOT NULL DEFAULT true,
    diagram_format VARCHAR(10) NOT NULL DEFAULT 'PNG',
    theme VARCHAR(50) NOT NULL DEFAULT 'Default',
    custom_settings VARCHAR(4000),
    include_code_examples BOOLEAN NOT NULL DEFAULT true,
    include_versioning BOOLEAN NOT NULL DEFAULT true,
    
    -- Metadata
    version VARCHAR(50) NOT NULL DEFAULT '1.0.0',
    last_analyzed_at TIMESTAMPTZ,
    
    -- Audit fields
    created_at TIMESTAMPTZ DEFAULT NOW() NOT NULL,
    updated_at TIMESTAMPTZ DEFAULT NOW() NOT NULL,
    created_by UUID REFERENCES auth.users(id) NOT NULL,
    updated_by UUID REFERENCES auth.users(id) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT true
);

-- Habilitar RLS en projects
ALTER TABLE autodocops.projects ENABLE ROW LEVEL SECURITY;

-- Políticas para projects
CREATE POLICY "Users can view own projects" ON autodocops.projects
    FOR SELECT USING (auth.uid() = created_by);

CREATE POLICY "Users can create projects" ON autodocops.projects
    FOR INSERT WITH CHECK (auth.uid() = created_by);

CREATE POLICY "Users can update own projects" ON autodocops.projects
    FOR UPDATE USING (auth.uid() = created_by);

CREATE POLICY "Users can delete own projects" ON autodocops.projects
    FOR DELETE USING (auth.uid() = created_by);

-- Crear tabla de documentación de APIs
CREATE TABLE IF NOT EXISTS autodocops.api_documentations (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    project_id UUID REFERENCES autodocops.projects(id) ON DELETE CASCADE NOT NULL,
    api_name VARCHAR(200) NOT NULL,
    version VARCHAR(50) NOT NULL,
    base_url VARCHAR(500) NOT NULL,
    description VARCHAR(2000) NOT NULL,
    language INTEGER NOT NULL, -- 1: Spanish, 2: English
    
    -- Documentation Content
    open_api_spec TEXT NOT NULL,
    postman_collection TEXT,
    typescript_sdk TEXT,
    csharp_sdk TEXT,
    usage_guides TEXT,
    metadata TEXT,
    
    -- Vector embeddings for semantic search
    embeddings vector(1536), -- OpenAI embeddings dimension
    
    -- Metadata
    last_generated_at TIMESTAMPTZ,
    
    -- Audit fields
    created_at TIMESTAMPTZ DEFAULT NOW() NOT NULL,
    updated_at TIMESTAMPTZ DEFAULT NOW() NOT NULL,
    created_by UUID REFERENCES auth.users(id) NOT NULL,
    updated_by UUID REFERENCES auth.users(id) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT true
);

-- Habilitar RLS en api_documentations
ALTER TABLE autodocops.api_documentations ENABLE ROW LEVEL SECURITY;

-- Políticas para api_documentations (heredan del proyecto)
CREATE POLICY "Users can view api docs from own projects" ON autodocops.api_documentations
    FOR SELECT USING (
        EXISTS (
            SELECT 1 FROM autodocops.projects p 
            WHERE p.id = project_id AND p.created_by = auth.uid()
        )
    );

CREATE POLICY "Users can create api docs for own projects" ON autodocops.api_documentations
    FOR INSERT WITH CHECK (
        EXISTS (
            SELECT 1 FROM autodocops.projects p 
            WHERE p.id = project_id AND p.created_by = auth.uid()
        )
    );

CREATE POLICY "Users can update api docs from own projects" ON autodocops.api_documentations
    FOR UPDATE USING (
        EXISTS (
            SELECT 1 FROM autodocops.projects p 
            WHERE p.id = project_id AND p.created_by = auth.uid()
        )
    );

CREATE POLICY "Users can delete api docs from own projects" ON autodocops.api_documentations
    FOR DELETE USING (
        EXISTS (
            SELECT 1 FROM autodocops.projects p 
            WHERE p.id = project_id AND p.created_by = auth.uid()
        )
    );

-- Crear tabla de esquemas de base de datos
CREATE TABLE IF NOT EXISTS autodocops.database_schemas (
    id UUID DEFAULT uuid_generate_v4() PRIMARY KEY,
    project_id UUID REFERENCES autodocops.projects(id) ON DELETE CASCADE NOT NULL,
    database_name VARCHAR(200) NOT NULL,
    schema_name VARCHAR(200) NOT NULL,
    version VARCHAR(50) NOT NULL,
    description VARCHAR(2000) NOT NULL,
    language INTEGER NOT NULL, -- 1: Spanish, 2: English
    
    -- Documentation Content
    schema_definition TEXT NOT NULL,
    er_diagram TEXT,
    data_dictionary TEXT,
    sample_queries TEXT,
    stored_procedures_doc TEXT,
    functions_doc TEXT,
    triggers_doc TEXT,
    usage_guides TEXT,
    metadata TEXT,
    
    -- Vector embeddings for semantic search
    embeddings vector(1536), -- OpenAI embeddings dimension
    
    -- Statistics
    table_count INTEGER NOT NULL DEFAULT 0,
    view_count INTEGER NOT NULL DEFAULT 0,
    stored_procedure_count INTEGER NOT NULL DEFAULT 0,
    function_count INTEGER NOT NULL DEFAULT 0,
    
    -- Metadata
    last_generated_at TIMESTAMPTZ,
    
    -- Audit fields
    created_at TIMESTAMPTZ DEFAULT NOW() NOT NULL,
    updated_at TIMESTAMPTZ DEFAULT NOW() NOT NULL,
    created_by UUID REFERENCES auth.users(id) NOT NULL,
    updated_by UUID REFERENCES auth.users(id) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT true
);

-- Habilitar RLS en database_schemas
ALTER TABLE autodocops.database_schemas ENABLE ROW LEVEL SECURITY;

-- Políticas para database_schemas (heredan del proyecto)
CREATE POLICY "Users can view db schemas from own projects" ON autodocops.database_schemas
    FOR SELECT USING (
        EXISTS (
            SELECT 1 FROM autodocops.projects p 
            WHERE p.id = project_id AND p.created_by = auth.uid()
        )
    );

CREATE POLICY "Users can create db schemas for own projects" ON autodocops.database_schemas
    FOR INSERT WITH CHECK (
        EXISTS (
            SELECT 1 FROM autodocops.projects p 
            WHERE p.id = project_id AND p.created_by = auth.uid()
        )
    );

CREATE POLICY "Users can update db schemas from own projects" ON autodocops.database_schemas
    FOR UPDATE USING (
        EXISTS (
            SELECT 1 FROM autodocops.projects p 
            WHERE p.id = project_id AND p.created_by = auth.uid()
        )
    );

CREATE POLICY "Users can delete db schemas from own projects" ON autodocops.database_schemas
    FOR DELETE USING (
        EXISTS (
            SELECT 1 FROM autodocops.projects p 
            WHERE p.id = project_id AND p.created_by = auth.uid()
        )
    );

-- Crear índices para optimizar consultas
CREATE INDEX IF NOT EXISTS idx_projects_name ON autodocops.projects(name);
CREATE INDEX IF NOT EXISTS idx_projects_type ON autodocops.projects(type);
CREATE INDEX IF NOT EXISTS idx_projects_status ON autodocops.projects(status);
CREATE INDEX IF NOT EXISTS idx_projects_created_by_created_at ON autodocops.projects(created_by, created_at);

CREATE INDEX IF NOT EXISTS idx_api_docs_project_id ON autodocops.api_documentations(project_id);
CREATE INDEX IF NOT EXISTS idx_api_docs_api_name ON autodocops.api_documentations(api_name);
CREATE INDEX IF NOT EXISTS idx_api_docs_version ON autodocops.api_documentations(version);

CREATE INDEX IF NOT EXISTS idx_db_schemas_project_id ON autodocops.database_schemas(project_id);
CREATE INDEX IF NOT EXISTS idx_db_schemas_database_name ON autodocops.database_schemas(database_name);
CREATE INDEX IF NOT EXISTS idx_db_schemas_schema_name ON autodocops.database_schemas(schema_name);
CREATE INDEX IF NOT EXISTS idx_db_schemas_database_schema ON autodocops.database_schemas(database_name, schema_name);

-- Crear índices vectoriales para búsqueda semántica
CREATE INDEX IF NOT EXISTS idx_api_docs_embeddings_ivfflat 
ON autodocops.api_documentations 
USING ivfflat (embeddings vector_cosine_ops)
WITH (lists = 100);

CREATE INDEX IF NOT EXISTS idx_db_schemas_embeddings_ivfflat 
ON autodocops.database_schemas 
USING ivfflat (embeddings vector_cosine_ops)
WITH (lists = 100);

-- Crear función para actualizar updated_at automáticamente
CREATE OR REPLACE FUNCTION autodocops.update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Crear triggers para actualizar updated_at
CREATE TRIGGER update_user_profiles_updated_at 
    BEFORE UPDATE ON autodocops.user_profiles 
    FOR EACH ROW EXECUTE FUNCTION autodocops.update_updated_at_column();

CREATE TRIGGER update_projects_updated_at 
    BEFORE UPDATE ON autodocops.projects 
    FOR EACH ROW EXECUTE FUNCTION autodocops.update_updated_at_column();

CREATE TRIGGER update_api_documentations_updated_at 
    BEFORE UPDATE ON autodocops.api_documentations 
    FOR EACH ROW EXECUTE FUNCTION autodocops.update_updated_at_column();

CREATE TRIGGER update_database_schemas_updated_at 
    BEFORE UPDATE ON autodocops.database_schemas 
    FOR EACH ROW EXECUTE FUNCTION autodocops.update_updated_at_column();

-- Crear función para búsqueda semántica
CREATE OR REPLACE FUNCTION autodocops.semantic_search(
    query_embedding vector(1536),
    match_threshold float DEFAULT 0.8,
    match_count int DEFAULT 10,
    project_filter uuid DEFAULT NULL
)
RETURNS TABLE (
    id uuid,
    content text,
    similarity float,
    type text,
    project_id uuid,
    metadata jsonb
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    (
        SELECT 
            a.id,
            a.open_api_spec as content,
            1 - (a.embeddings <=> query_embedding) as similarity,
            'api'::text as type,
            a.project_id,
            jsonb_build_object(
                'api_name', a.api_name,
                'version', a.version,
                'base_url', a.base_url
            ) as metadata
        FROM autodocops.api_documentations a
        WHERE 
            (project_filter IS NULL OR a.project_id = project_filter)
            AND a.is_active = true
            AND a.embeddings IS NOT NULL
            AND 1 - (a.embeddings <=> query_embedding) > match_threshold
        ORDER BY a.embeddings <=> query_embedding
        LIMIT match_count
    )
    UNION ALL
    (
        SELECT 
            d.id,
            d.schema_definition as content,
            1 - (d.embeddings <=> query_embedding) as similarity,
            'database'::text as type,
            d.project_id,
            jsonb_build_object(
                'database_name', d.database_name,
                'schema_name', d.schema_name,
                'version', d.version
            ) as metadata
        FROM autodocops.database_schemas d
        WHERE 
            (project_filter IS NULL OR d.project_id = project_filter)
            AND d.is_active = true
            AND d.embeddings IS NOT NULL
            AND 1 - (d.embeddings <=> query_embedding) > match_threshold
        ORDER BY d.embeddings <=> query_embedding
        LIMIT match_count
    )
    ORDER BY similarity DESC
    LIMIT match_count;
END;
$$;

-- Crear función para insertar perfil de usuario automáticamente
CREATE OR REPLACE FUNCTION autodocops.handle_new_user()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO autodocops.user_profiles (id, email, name)
    VALUES (
        NEW.id,
        NEW.email,
        COALESCE(NEW.raw_user_meta_data->>'name', NEW.email)
    );
    RETURN NEW;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

-- Crear trigger para nuevos usuarios
CREATE TRIGGER on_auth_user_created
    AFTER INSERT ON auth.users
    FOR EACH ROW EXECUTE FUNCTION autodocops.handle_new_user();

-- Insertar datos de ejemplo (opcional para desarrollo)
-- Nota: En producción, estos datos se crearán a través de la aplicación

COMMENT ON SCHEMA autodocops IS 'Esquema principal para AutoDocOps - Generador automático de documentación';
COMMENT ON TABLE autodocops.projects IS 'Proyectos de documentación de APIs y bases de datos';
COMMENT ON TABLE autodocops.api_documentations IS 'Documentación generada para APIs .NET';
COMMENT ON TABLE autodocops.database_schemas IS 'Documentación generada para esquemas de base de datos';
COMMENT ON FUNCTION autodocops.semantic_search IS 'Función de búsqueda semántica usando embeddings vectoriales';

