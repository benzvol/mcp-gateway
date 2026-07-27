<script setup lang="ts">
interface NavItem {
  label: string
  routeName?: string
  disabled?: boolean
}

interface NavGroup {
  label: string
  items: NavItem[]
}

const groups: NavGroup[] = [
  {
    label: 'Overview',
    items: [
      { label: 'Dashboard', disabled: true },
      { label: 'Audit log', disabled: true },
    ],
  },
  {
    label: 'Configure',
    items: [
      { label: 'Upstreams', routeName: 'upstreams' },
      { label: 'Tools catalog', disabled: true },
      { label: 'Router mode', disabled: true },
      { label: 'Clients & sync', disabled: true },
    ],
  },
]
</script>

<template>
  <aside class="w-59 flex-none border-r border-border pb-4">
    <div class="border-b border-border px-4.5 py-5 font-bold">MCP Gateway</div>
    <nav v-for="group in groups" :key="group.label" class="flex flex-col px-3 py-2">
      <div class="px-2.5 pt-4 pb-1.5 text-[0.7rem] uppercase tracking-wider text-muted-foreground">
        {{ group.label }}
      </div>
      <template v-for="item in group.items" :key="item.label">
        <RouterLink
          v-if="item.routeName"
          :to="{ name: item.routeName }"
          class="rounded-md px-2.5 py-2 text-[0.85rem] text-muted-foreground no-underline hover:bg-accent [&.router-link-active]:bg-accent [&.router-link-active]:text-accent-foreground"
        >
          {{ item.label }}
        </RouterLink>
        <span
          v-else
          class="pointer-events-none px-2.5 py-2 text-[0.85rem] text-muted-foreground opacity-50"
        >
          {{ item.label }}
        </span>
      </template>
    </nav>
  </aside>
</template>
