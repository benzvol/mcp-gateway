export const upstreamKeys = {
    all: ['upstreams'] as const,
    lists: () => [...upstreamKeys.all, 'list'] as const,
    detail: (id: string) => [...upstreamKeys.all, 'detail', id] as const,
}
