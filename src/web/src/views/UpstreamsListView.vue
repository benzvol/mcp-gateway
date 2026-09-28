<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { toast } from 'vue-sonner'

import { Button } from '@/components/ui/button'
import UpstreamTable from '@/components/upstreams/UpstreamTable.vue'
import ConfirmDeleteDialog from '@/components/common/ConfirmDeleteDialog.vue'
import { useDeleteUpstream, useSetEnabled, useUpstreamsQuery } from '@/queries/upstreams'
import type { UpstreamResponse } from '@/types/upstream'

const router = useRouter()

const { data: upstreams, isLoading } = useUpstreamsQuery()
const setEnabled = useSetEnabled()
const deleteUpstream = useDeleteUpstream()

const pendingDelete = ref<UpstreamResponse | null>(null)
const deleteDialogOpen = ref(false)

function toggle(id: string, enabled: boolean) {
  setEnabled.mutate({ id, enabled })
}

function requestDelete(upstream: UpstreamResponse) {
  pendingDelete.value = upstream
  deleteDialogOpen.value = true
}

function confirmDelete() {
  if (!pendingDelete.value) return
  const { id, name } = pendingDelete.value
  deleteUpstream.mutate(id, {
    onSuccess: () => toast.success(`Deleted ${name}`),
    onError: () => toast.error(`Failed to delete ${name}`),
  })
}
</script>

<template>
  <div class="mb-6 flex items-start justify-between gap-6">
    <div>
      <h1 class="mb-1.5 text-2xl font-bold">Upstream servers</h1>
      <p class="text-muted-foreground">The MCP servers patched into the gateway.</p>
    </div>
    <Button @click="router.push({ name: 'upstream-create' })">Add upstream</Button>
  </div>

  <UpstreamTable
    :upstreams="upstreams ?? []"
    :loading="isLoading"
    @toggle="toggle"
    @delete="requestDelete"
  />

  <ConfirmDeleteDialog
    v-model:open="deleteDialogOpen"
    :name="pendingDelete?.name ?? ''"
    :loading="deleteUpstream.isPending.value"
    @confirm="confirmDelete"
  />
</template>
