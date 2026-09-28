import { ref } from 'vue'

export interface KeyValueRow {
    id: number
    key: string
    value: string
}

let nextId = 0

export function useKeyValueList(initial: Record<string, string> = {}) {
    const rows = ref<KeyValueRow[]>(
        Object.entries(initial).map(([key, value]) => ({ id: nextId++, key, value })),
    )

    function add() {
        rows.value.push({ id: nextId++, key: '', value: '' })
    }

    function remove(id: number) {
        rows.value = rows.value.filter((r) => r.id !== id)
    }

    function reset(next: Record<string, string> = {}) {
        rows.value = Object.entries(next).map(([key, value]) => ({ id: nextId++, key, value }))
    }

    function toRecord(): Record<string, string> {
        const record: Record<string, string> = {}
        for (const row of rows.value) {
            const key = row.key.trim()
            if (key.length === 0) continue
            record[key] = row.value
        }
        return record
    }

    return { rows, add, remove, reset, toRecord }
}
