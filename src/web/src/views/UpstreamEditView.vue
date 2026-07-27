<script setup lang="ts">
import { LoaderCircleIcon } from '@lucide/vue'

import UpstreamForm from '@/components/upstreams/UpstreamForm.vue'
import { useUpstreamQuery } from '@/queries/upstreams'

const props = defineProps<{ id: string }>()

const { data: upstream, isLoading } = useUpstreamQuery(() => props.id)
</script>

<template>
  <LoaderCircleIcon v-if="isLoading" class="size-6 animate-spin text-muted-foreground" />
  <UpstreamForm v-else-if="upstream" :initial="upstream" />
</template>
