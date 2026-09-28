<script setup lang="ts">
import { XIcon } from '@lucide/vue'

import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { type StringRow } from '@/composables/useStringList'

const props = defineProps<{
  rows: StringRow[]
  placeholder?: string
}>()

const emit = defineEmits<{
  add: []
  remove: [id: number]
}>()
</script>

<template>
  <div class="flex flex-col gap-2">
    <div
      v-for="row in props.rows"
      :key="row.id"
      class="grid grid-cols-[1fr_auto] items-center gap-2"
    >
      <Input v-model="row.value" :placeholder="placeholder ?? 'Value'" />
      <Button variant="ghost" size="icon" type="button" @click="emit('remove', row.id)">
        <XIcon class="size-4" />
      </Button>
    </div>
    <Button variant="outline" size="sm" type="button" class="self-start" @click="emit('add')"
      >Add</Button
    >
  </div>
</template>
