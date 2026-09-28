import { useMutation } from '@tanstack/vue-query'

import { upstreamsApi } from '@/api/upstreams'
import type { CreateUpstreamRequest } from '@/types/upstream'

// Test results are transient — held in the caller's local state, never
// written to the vue-query cache.
export function useTestUnsavedConnection() {
    return useMutation({
        mutationFn: (body: CreateUpstreamRequest) => upstreamsApi.testUnsaved(body),
    })
}

export function useTestSavedConnection() {
    return useMutation({
        mutationFn: (id: string) => upstreamsApi.testSaved(id),
    })
}
