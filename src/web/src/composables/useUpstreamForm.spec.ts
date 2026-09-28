import { describe, expect, it } from 'vitest'

import { useUpstreamForm } from './useUpstreamForm'
import type { UpstreamResponse } from '@/types/upstream'

function stdioUpstream(overrides: Partial<UpstreamResponse> = {}): UpstreamResponse {
    return {
        id: 'u1',
        name: 'filesystem',
        enabled: true,
        transport: 'Stdio',
        command: 'npx',
        args: ['-y', '@modelcontextprotocol/server-filesystem'],
        environment: {},
        headers: {},
        authKind: 'ApiKey',
        hasAuthConfig: false,
        hasSecret: true,
        ...overrides,
    }
}

describe('useUpstreamForm secret preservation', () => {
    it('omits secret when untouched on edit (preserves stored value)', () => {
        const form = useUpstreamForm(stdioUpstream())

        const request = form.toUpdateRequest()

        expect(request.secret).toBeUndefined()
        expect('secret' in JSON.parse(JSON.stringify(request))).toBe(false)
    })

    it('sends the typed value when the secret field is dirty', () => {
        const form = useUpstreamForm(stdioUpstream())

        form.markSecretDirty('sk-live-new')
        const request = form.toUpdateRequest()

        expect(request.secret).toBe('sk-live-new')
    })

    it('sends empty string when the secret is explicitly cleared', () => {
        const form = useUpstreamForm(stdioUpstream())

        form.clearSecret()
        const request = form.toUpdateRequest()

        expect(request.secret).toBe('')
    })

    it('omits authConfigJson when untouched on edit', () => {
        const form = useUpstreamForm(stdioUpstream({ authKind: 'OAuth2', hasAuthConfig: true }))

        const request = form.toUpdateRequest()

        expect(request.authConfigJson).toBeUndefined()
        expect('authConfigJson' in JSON.parse(JSON.stringify(request))).toBe(false)
    })

    it('sends the typed value when authConfigJson is dirty', () => {
        const form = useUpstreamForm(stdioUpstream({ authKind: 'OAuth2', hasAuthConfig: true }))

        form.markAuthConfigDirty('{"header":"X-Api-Key"}')
        const request = form.toUpdateRequest()

        expect(request.authConfigJson).toBe('{"header":"X-Api-Key"}')
    })

    it('sends empty string when authConfigJson is explicitly cleared', () => {
        const form = useUpstreamForm(stdioUpstream({ authKind: 'OAuth2', hasAuthConfig: true }))

        form.clearAuthConfig()
        const request = form.toUpdateRequest()

        expect(request.authConfigJson).toBe('')
    })

    it('never sends a secret/authConfigJson change for a fresh create form', () => {
        const form = useUpstreamForm()

        const request = form.toCreateRequest()

        expect(request.secret).toBeNull()
        expect(request.authConfigJson).toBeNull()
    })
})

describe('useUpstreamForm transport-conditional fields', () => {
    it('excludes stdio fields when transport is StreamableHttp', () => {
        const form = useUpstreamForm(
            stdioUpstream({
                transport: 'StreamableHttp',
                endpoint: 'https://upstream.example.com/mcp',
            }),
        )

        const request = form.toUpdateRequest()

        expect(request.command).toBeUndefined()
        expect(request.args).toBeUndefined()
        expect(request.environment).toBeUndefined()
        expect(request.endpoint).toBe('https://upstream.example.com/mcp')
    })

    it('excludes http fields when transport is Stdio', () => {
        const form = useUpstreamForm(stdioUpstream())

        const request = form.toUpdateRequest()

        expect(request.endpoint).toBeUndefined()
        expect(request.command).toBe('npx')
    })
})

describe('useUpstreamForm auth-conditional fields', () => {
    it('only includes secret when authKind is ApiKey', () => {
        const form = useUpstreamForm(stdioUpstream({ authKind: 'None' }))

        form.markSecretDirty('should-not-be-sent')
        const request = form.toUpdateRequest()

        expect(request.secret).toBeNull()
    })
})
