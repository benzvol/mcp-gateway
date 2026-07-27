import { describe, expect, it } from 'vitest'

import { authKindLabels, failureKindLabels, transportLabels } from './labels'
import type { AuthKind, FailureKind, TransportKind } from './upstream'

const transportKinds: TransportKind[] = ['Stdio', 'StreamableHttp']
const authKinds: AuthKind[] = ['None', 'ApiKey', 'OAuth2', 'CustomHeaders']
const failureKinds: FailureKind[] = [
    'None',
    'Configuration',
    'Unreachable',
    'AuthFailed',
    'Timeout',
    'Protocol',
    'Unknown',
]

describe('label maps', () => {
    it('has a non-empty label for every TransportKind member', () => {
        for (const kind of transportKinds) {
            expect(transportLabels[kind]).toBeTruthy()
        }
    })

    it('has a non-empty label for every AuthKind member', () => {
        for (const kind of authKinds) {
            expect(authKindLabels[kind]).toBeTruthy()
        }
    })

    it('has a non-empty label for every FailureKind member', () => {
        for (const kind of failureKinds) {
            expect(failureKindLabels[kind]).toBeTruthy()
        }
    })
})
