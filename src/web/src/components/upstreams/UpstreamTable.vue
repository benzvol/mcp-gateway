<script setup lang="ts">
import { useRouter } from 'vue-router'

import { Button } from '@/components/ui/button'
import { Switch } from '@/components/ui/switch'
import {
  Table,
  TableBody,
  TableCell,
  TableEmpty,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import UpstreamStatusTag from './UpstreamStatusTag.vue'
import { authKindLabels, transportLabels } from '@/types/labels'
import type { UpstreamResponse } from '@/types/upstream'

const props = defineProps<{
  upstreams: UpstreamResponse[]
  loading?: boolean
}>()

const emit = defineEmits<{
  toggle: [id: string, enabled: boolean]
  delete: [upstream: UpstreamResponse]
}>()

const router = useRouter()

function edit(id: string) {
  router.push({ name: 'upstream-edit', params: { id } })
}
</script>

<template>
  <Table>
    <TableHeader>
      <TableRow>
        <TableHead>Name</TableHead>
        <TableHead>Transport</TableHead>
        <TableHead>Auth</TableHead>
        <TableHead>Status</TableHead>
        <TableHead class="w-24">Patched in</TableHead>
        <TableHead class="w-40" />
      </TableRow>
    </TableHeader>
    <TableBody>
      <TableEmpty v-if="!props.loading && props.upstreams.length === 0" :colspan="6">
        No upstreams yet. Add one to get started.
      </TableEmpty>
      <TableRow v-for="upstream in props.upstreams" :key="upstream.id">
        <TableCell class="font-mono">{{ upstream.name }}</TableCell>
        <TableCell>{{ transportLabels[upstream.transport] }}</TableCell>
        <TableCell>{{ authKindLabels[upstream.authKind] }}</TableCell>
        <TableCell>
          <UpstreamStatusTag :enabled="upstream.enabled" />
        </TableCell>
        <TableCell>
          <Switch
            :model-value="upstream.enabled"
            @update:model-value="(v) => emit('toggle', upstream.id, v)"
          />
        </TableCell>
        <TableCell class="text-right">
          <div class="flex justify-end gap-1">
            <Button variant="ghost" size="sm" @click="edit(upstream.id)">Edit</Button>
            <Button
              variant="ghost"
              size="sm"
              class="text-destructive"
              @click="emit('delete', upstream)"
            >
              Delete
            </Button>
          </div>
        </TableCell>
      </TableRow>
    </TableBody>
  </Table>
</template>
