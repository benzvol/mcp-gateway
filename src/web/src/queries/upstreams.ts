import { computed, type MaybeRefOrGetter, toValue } from 'vue'
import { useMutation, useQuery, useQueryClient } from '@tanstack/vue-query'

import { upstreamsApi } from '@/api/upstreams'
import type {
    CreateUpstreamRequest,
    UpdateUpstreamRequest,
    UpstreamResponse,
} from '@/types/upstream'

import { upstreamKeys } from './keys'

export function useUpstreamsQuery() {
    return useQuery({
        queryKey: upstreamKeys.lists(),
        queryFn: ({ signal }) => upstreamsApi.list(signal),
        staleTime: 10_000,
    })
}

export function useUpstreamQuery(id: MaybeRefOrGetter<string | undefined>) {
    return useQuery({
        queryKey: computed(() => upstreamKeys.detail(toValue(id) ?? '')),
        queryFn: ({ signal }) => upstreamsApi.get(toValue(id)!, signal),
        enabled: computed(() => !!toValue(id)),
    })
}

export function useCreateUpstream() {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: (body: CreateUpstreamRequest) => upstreamsApi.create(body),
        onSuccess: () => {
            void queryClient.invalidateQueries({ queryKey: upstreamKeys.lists() })
        },
    })
}

export function useUpdateUpstream() {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: ({ id, body }: { id: string; body: UpdateUpstreamRequest }) =>
            upstreamsApi.update(id, body),
        onSuccess: (_, { id }) => {
            void queryClient.invalidateQueries({ queryKey: upstreamKeys.detail(id) })
            void queryClient.invalidateQueries({ queryKey: upstreamKeys.lists() })
        },
    })
}

export function useDeleteUpstream() {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: (id: string) => upstreamsApi.remove(id),
        onMutate: async (id) => {
            await queryClient.cancelQueries({ queryKey: upstreamKeys.lists() })
            const previous = queryClient.getQueryData<UpstreamResponse[]>(upstreamKeys.lists())
            queryClient.setQueryData<UpstreamResponse[]>(upstreamKeys.lists(), (old) =>
                old?.filter((u) => u.id !== id),
            )
            return { previous }
        },
        onError: (_err, _id, context) => {
            if (context?.previous) {
                queryClient.setQueryData(upstreamKeys.lists(), context.previous)
            }
        },
        onSettled: () => {
            void queryClient.invalidateQueries({ queryKey: upstreamKeys.lists() })
        },
    })
}

export function useSetEnabled() {
    const queryClient = useQueryClient()
    return useMutation({
        mutationFn: ({ id, enabled }: { id: string; enabled: boolean }) =>
            upstreamsApi.setEnabled(id, enabled),
        onMutate: async ({ id, enabled }) => {
            await queryClient.cancelQueries({ queryKey: upstreamKeys.lists() })
            const previous = queryClient.getQueryData<UpstreamResponse[]>(upstreamKeys.lists())
            queryClient.setQueryData<UpstreamResponse[]>(upstreamKeys.lists(), (old) =>
                old?.map((u) => (u.id === id ? { ...u, enabled } : u)),
            )
            return { previous }
        },
        onError: (_err, _vars, context) => {
            if (context?.previous) {
                queryClient.setQueryData(upstreamKeys.lists(), context.previous)
            }
        },
        onSettled: (_data, _err, { id }) => {
            void queryClient.invalidateQueries({ queryKey: upstreamKeys.lists() })
            void queryClient.invalidateQueries({ queryKey: upstreamKeys.detail(id) })
        },
    })
}
