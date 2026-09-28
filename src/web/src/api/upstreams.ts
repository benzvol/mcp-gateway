import { http } from '@/lib/http'
import type {
    CreateUpstreamRequest,
    TestConnectionResponse,
    UpdateUpstreamRequest,
    UpstreamResponse,
} from '@/types/upstream'

export const upstreamsApi = {
    list: (signal?: AbortSignal) => http.get<UpstreamResponse[]>('/upstreams', signal),

    get: (id: string, signal?: AbortSignal) =>
        http.get<UpstreamResponse>(`/upstreams/${id}`, signal),

    create: (body: CreateUpstreamRequest) => http.post<UpstreamResponse>('/upstreams', body),

    update: (id: string, body: UpdateUpstreamRequest) =>
        http.put<UpstreamResponse>(`/upstreams/${id}`, body),

    remove: (id: string) => http.delete<void>(`/upstreams/${id}`),

    setEnabled: (id: string, enabled: boolean) =>
        http.post<void>(`/upstreams/${id}/enabled`, { enabled }),

    testUnsaved: (body: CreateUpstreamRequest) =>
        http.post<TestConnectionResponse>('/upstreams/test', body),

    testSaved: (id: string) => http.post<TestConnectionResponse>(`/upstreams/${id}/test`),
}
