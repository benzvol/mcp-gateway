import { ref } from 'vue'

import { ConflictError, ValidationError } from '@/lib/api'

export function useFieldErrors() {
    const fieldErrors = ref<Record<string, string[]>>({})
    const conflictMessage = ref<string | undefined>()

    function apply(err: unknown) {
        fieldErrors.value = {}
        conflictMessage.value = undefined

        if (err instanceof ValidationError) {
            fieldErrors.value = err.problem.errors
        } else if (err instanceof ConflictError) {
            conflictMessage.value = err.detail
        } else {
            throw err
        }
    }

    function errorFor(field: string): string | undefined {
        return fieldErrors.value[field]?.[0]
    }

    function clear() {
        fieldErrors.value = {}
        conflictMessage.value = undefined
    }

    return { fieldErrors, conflictMessage, apply, errorFor, clear }
}
