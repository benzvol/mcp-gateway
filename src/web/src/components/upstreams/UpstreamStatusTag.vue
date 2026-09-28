<script setup lang="ts">
import { Badge } from '@/components/ui/badge'

defineProps<{ enabled: boolean }>()
</script>

<template>
  <Badge :variant="enabled ? 'default' : 'secondary'">{{ enabled ? 'Enabled' : 'Disabled' }}</Badge>
</template>
