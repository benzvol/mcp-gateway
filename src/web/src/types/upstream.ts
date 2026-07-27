export type TransportKind = 'Stdio' | 'StreamableHttp'

export type AuthKind = 'None' | 'ApiKey' | 'OAuth2' | 'CustomHeaders'

export type FailureKind =
    'None' | 'Configuration' | 'Unreachable' | 'AuthFailed' | 'Timeout' | 'Protocol' | 'Unknown'

export interface UpstreamResponse {
    id: string
    name: string
    enabled: boolean
    transport: TransportKind
    command?: string
    args: string[]
    environment: Record<string, string>
    endpoint?: string
    headers: Record<string, string>
    authKind: AuthKind
    hasAuthConfig: boolean
    hasSecret: boolean
}

export interface CreateUpstreamRequest {
    name: string
    transport: TransportKind
    command?: string
    args?: string[]
    environment?: Record<string, string>
    endpoint?: string
    headers?: Record<string, string>
    authKind: AuthKind
    authConfigJson?: string | null
    secret?: string | null
    enabled?: boolean
}

export interface UpdateUpstreamRequest extends Omit<CreateUpstreamRequest, 'enabled'> {
    enabled: boolean
}

export interface ToolPreview {
    name: string
    description?: string
}

export interface TestConnectionResponse {
    success: boolean
    failureKind: FailureKind
    message?: string
    latencyMs: number
    tools: ToolPreview[]
}
