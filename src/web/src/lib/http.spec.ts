import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

import { ApiError, ConflictError, NotFoundError, ValidationError } from './api'
import { http } from './http'

function jsonResponse(body: unknown, status = 200) {
    return new Response(JSON.stringify(body), {
        status,
        headers: { 'Content-Type': 'application/json' },
    })
}

describe('http', () => {
    beforeEach(() => {
        vi.stubGlobal('fetch', vi.fn())
    })

    afterEach(() => {
        vi.unstubAllGlobals()
    })

    it('resolves undefined on 204 No Content', async () => {
        vi.mocked(fetch).mockResolvedValue(new Response(null, { status: 204 }))

        const result = await http.delete('/upstreams/1')

        expect(result).toBeUndefined()
    })

    it('resolves the parsed body on 200', async () => {
        vi.mocked(fetch).mockResolvedValue(jsonResponse({ id: '1', name: 'filesystem' }))

        const result = await http.get<{ id: string; name: string }>('/upstreams/1')

        expect(result).toEqual({ id: '1', name: 'filesystem' })
    })

    it('throws ValidationError with parsed problem errors on 400', async () => {
        vi.mocked(fetch).mockResolvedValue(
            jsonResponse(
                {
                    title: 'Validation failed',
                    errors: { command: ['A stdio upstream requires a command.'] },
                },
                400,
            ),
        )

        await expect(http.post('/upstreams', {})).rejects.toSatisfy((err: unknown) => {
            expect(err).toBeInstanceOf(ValidationError)
            expect((err as ValidationError).problem.errors.command).toEqual([
                'A stdio upstream requires a command.',
            ])
            return true
        })
    })

    it('throws ConflictError with the plain-text body on 409', async () => {
        vi.mocked(fetch).mockResolvedValue(
            new Response("An upstream named 'filesystem' already exists.", { status: 409 }),
        )

        await expect(http.post('/upstreams', {})).rejects.toSatisfy((err: unknown) => {
            expect(err).toBeInstanceOf(ConflictError)
            expect((err as ConflictError).detail).toBe(
                "An upstream named 'filesystem' already exists.",
            )
            return true
        })
    })

    it('throws NotFoundError on 404', async () => {
        vi.mocked(fetch).mockResolvedValue(new Response(null, { status: 404 }))

        await expect(http.get('/upstreams/missing')).rejects.toBeInstanceOf(NotFoundError)
    })

    it('throws ApiError for other non-ok statuses', async () => {
        vi.mocked(fetch).mockResolvedValue(
            new Response(null, { status: 500, statusText: 'Internal Server Error' }),
        )

        await expect(http.get('/upstreams')).rejects.toBeInstanceOf(ApiError)
    })
})
