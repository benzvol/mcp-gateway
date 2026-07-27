import { computed, reactive } from 'vue'

import { useKeyValueList } from './useKeyValueList'
import { useStringList } from './useStringList'
import type {
    AuthKind,
    CreateUpstreamRequest,
    TransportKind,
    UpdateUpstreamRequest,
    UpstreamResponse,
} from '@/types/upstream'

interface SecretModel {
    input: string
    dirty: boolean
    clear: boolean
}

function newSecretModel(): SecretModel {
    return { input: '', dirty: false, clear: false }
}

// Backend preservation semantics (UpstreamMapping.Apply): omitting secret/
// authConfigJson from the request keeps the stored value; sending '' clears
// it; sending a non-empty value replaces it. The dirty/clear flags below
// exist so a save never silently wipes a stored credential the user didn't
// touch.
function resolveSecretField(model: SecretModel): string | null | undefined {
    if (model.clear) return ''
    if (model.dirty) return model.input
    return undefined
}

export function useUpstreamForm(initial?: UpstreamResponse) {
    const isEdit = !!initial

    const state = reactive({
        name: initial?.name ?? '',
        enabled: initial?.enabled ?? true,
        transport: (initial?.transport ?? 'Stdio') as TransportKind,
        command: initial?.command ?? '',
        endpoint: initial?.endpoint ?? '',
        authKind: (initial?.authKind ?? 'None') as AuthKind,
    })

    const args = useStringList(initial?.args ?? [])
    const environment = useKeyValueList(initial?.environment ?? {})
    const headers = useKeyValueList(initial?.headers ?? {})

    const secret = reactive<SecretModel>(newSecretModel())
    const authConfigJson = reactive<SecretModel>(newSecretModel())

    const hasStoredSecret = computed(() => initial?.hasSecret ?? false)
    const hasStoredAuthConfig = computed(() => initial?.hasAuthConfig ?? false)

    const showStdioFields = computed(() => state.transport === 'Stdio')
    const showHttpFields = computed(() => state.transport === 'StreamableHttp')
    const showSecretField = computed(() => state.authKind === 'ApiKey')
    const showAuthConfigField = computed(() => state.authKind === 'OAuth2')
    const showCustomHeaders = computed(() => state.authKind === 'CustomHeaders')

    function buildBase(): Omit<CreateUpstreamRequest, 'enabled'> {
        return {
            name: state.name,
            transport: state.transport,
            command: showStdioFields.value ? state.command || undefined : undefined,
            args: showStdioFields.value ? args.toArray() : undefined,
            environment: showStdioFields.value ? environment.toRecord() : undefined,
            endpoint: showHttpFields.value ? state.endpoint || undefined : undefined,
            headers:
                showHttpFields.value || showCustomHeaders.value ? headers.toRecord() : undefined,
            authKind: state.authKind,
            secret: showSecretField.value ? resolveSecretField(secret) : null,
            authConfigJson: showAuthConfigField.value ? resolveSecretField(authConfigJson) : null,
        }
    }

    function toCreateRequest(): CreateUpstreamRequest {
        return { ...buildBase(), enabled: state.enabled }
    }

    function toUpdateRequest(): UpdateUpstreamRequest {
        return { ...buildBase(), enabled: state.enabled }
    }

    function markSecretDirty(value: string) {
        secret.input = value
        secret.dirty = true
        secret.clear = false
    }

    function clearSecret() {
        secret.input = ''
        secret.dirty = false
        secret.clear = true
    }

    function markAuthConfigDirty(value: string) {
        authConfigJson.input = value
        authConfigJson.dirty = true
        authConfigJson.clear = false
    }

    function clearAuthConfig() {
        authConfigJson.input = ''
        authConfigJson.dirty = false
        authConfigJson.clear = true
    }

    return {
        isEdit,
        state,
        args,
        environment,
        headers,
        secret,
        authConfigJson,
        hasStoredSecret,
        hasStoredAuthConfig,
        showStdioFields,
        showHttpFields,
        showSecretField,
        showAuthConfigField,
        showCustomHeaders,
        markSecretDirty,
        clearSecret,
        markAuthConfigDirty,
        clearAuthConfig,
        toCreateRequest,
        toUpdateRequest,
    }
}
