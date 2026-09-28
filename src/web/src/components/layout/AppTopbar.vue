<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()
const crumbs = computed(() => (route.meta.crumbs as string | undefined) ?? '')
</script>

<template>
  <header class="flex items-center justify-between border-b border-border px-7 py-3.5">
    <div class="text-sm text-muted-foreground">{{ crumbs }}</div>
    <div class="rounded-full border border-border px-3 py-1 text-xs text-muted-foreground">
      SQLite · localhost
    </div>
  </header>
</template>
