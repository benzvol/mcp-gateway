import type { AuthKind, FailureKind, TransportKind } from './upstream'

export const transportLabels: Record<TransportKind, string> = {
    Stdio: 'stdio',
    StreamableHttp: 'Streamable HTTP',
}

export const transportOptions = Object.entries(transportLabels).map(([value, label]) => ({
    value: value as TransportKind,
    label,
}))

export const authKindLabels: Record<AuthKind, string> = {
    None: 'None',
    ApiKey: 'API key',
    OAuth2: 'OAuth2',
    CustomHeaders: 'Custom headers',
}

export const authKindOptions = Object.entries(authKindLabels).map(([value, label]) => ({
    value: value as AuthKind,
    label,
}))

export const failureKindLabels: Record<FailureKind, string> = {
    None: 'No error',
    Configuration: 'Invalid configuration',
    Unreachable: 'Unreachable',
    AuthFailed: 'Authentication failed',
    Timeout: 'Timed out',
    Protocol: 'Protocol error',
    Unknown: 'Unknown error',
}
