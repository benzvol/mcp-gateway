import { describe, expect, it } from 'vitest'

import { useFieldErrors } from './useFieldErrors'
import { ConflictError, ValidationError } from '@/lib/api'

describe('useFieldErrors', () => {
    it('maps a ValidationError into per-field messages', () => {
        const fieldErrors = useFieldErrors()

        fieldErrors.apply(
            new ValidationError({
                title: 'Validation failed',
                errors: { command: ['A stdio upstream requires a command.'] },
            }),
        )

        expect(fieldErrors.errorFor('command')).toBe('A stdio upstream requires a command.')
        expect(fieldErrors.conflictMessage.value).toBeUndefined()
    })

    it('maps a ConflictError into a conflict message', () => {
        const fieldErrors = useFieldErrors()

        fieldErrors.apply(new ConflictError("An upstream named 'filesystem' already exists."))

        expect(fieldErrors.conflictMessage.value).toBe(
            "An upstream named 'filesystem' already exists.",
        )
        expect(fieldErrors.errorFor('name')).toBeUndefined()
    })

    it('clears previous errors before applying a new one', () => {
        const fieldErrors = useFieldErrors()
        fieldErrors.apply(new ValidationError({ errors: { endpoint: ['required'] } }))

        fieldErrors.apply(new ConflictError('duplicate name'))

        expect(fieldErrors.errorFor('endpoint')).toBeUndefined()
        expect(fieldErrors.conflictMessage.value).toBe('duplicate name')
    })

    it('rethrows unrecognized errors', () => {
        const fieldErrors = useFieldErrors()

        expect(() => fieldErrors.apply(new Error('boom'))).toThrow('boom')
    })

    it('clear() resets both field errors and the conflict message', () => {
        const fieldErrors = useFieldErrors()
        fieldErrors.apply(new ConflictError('duplicate name'))

        fieldErrors.clear()

        expect(fieldErrors.conflictMessage.value).toBeUndefined()
        expect(Object.keys(fieldErrors.fieldErrors.value)).toHaveLength(0)
    })
})
