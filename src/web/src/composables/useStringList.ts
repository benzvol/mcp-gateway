import { ref } from 'vue'

export interface StringRow {
    id: number
    value: string
}

let nextId = 0

export function useStringList(initial: string[] = []) {
    const rows = ref<StringRow[]>(initial.map((value) => ({ id: nextId++, value })))

    function add() {
        rows.value.push({ id: nextId++, value: '' })
    }

    function remove(id: number) {
        rows.value = rows.value.filter((r) => r.id !== id)
    }

    function reset(next: string[] = []) {
        rows.value = next.map((value) => ({ id: nextId++, value }))
    }

    function toArray(): string[] {
        return rows.value.map((r) => r.value).filter((v) => v.trim().length > 0)
    }

    return { rows, add, remove, reset, toArray }
}
