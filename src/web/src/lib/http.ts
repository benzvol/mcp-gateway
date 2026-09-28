import {
    ApiError,
    ConflictError,
    NotFoundError,
    ValidationError,
    type ValidationProblem,
} from './api'

const BASE = import.meta.env.VITE_API_BASE ?? '/api'

interface RequestOptions {
    method?: string
    body?: unknown
    signal?: AbortSignal
}

async function request<T>(path: string, opts: RequestOptions = {}): Promise<T> {
    const res = await fetch(`${BASE}${path}`, {
        method: opts.method ?? 'GET',
        headers: opts.body !== undefined ? { 'Content-Type': 'application/json' } : undefined,
        body: opts.body !== undefined ? JSON.stringify(opts.body) : undefined,
        signal: opts.signal,
    })

    if (res.status === 204 || res.headers.get('content-length') === '0') {
        return undefined as T
    }

    if (res.status === 404) {
        throw new NotFoundError()
    }

    if (res.status === 400) {
        const problem: ValidationProblem = await res.json()
        throw new ValidationError(problem)
    }

    if (res.status === 409) {
        const detail = await res.text()
        throw new ConflictError(detail)
    }

    if (!res.ok) {
        throw new ApiError(res.status, res.statusText)
    }

    return (await res.json()) as T
}

export const http = {
    get: <T>(path: string, signal?: AbortSignal) => request<T>(path, { signal }),
    post: <T>(path: string, body?: unknown, signal?: AbortSignal) =>
        request<T>(path, { method: 'POST', body, signal }),
    put: <T>(path: string, body?: unknown, signal?: AbortSignal) =>
        request<T>(path, { method: 'PUT', body, signal }),
    delete: <T>(path: string, signal?: AbortSignal) =>
        request<T>(path, { method: 'DELETE', signal }),
}
