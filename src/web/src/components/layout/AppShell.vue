<script setup lang="ts">
import AppSidebar from './AppSidebar.vue'
import AppTopbar from './AppTopbar.vue'
</script>

<template>
  <div class="flex min-h-screen">
    <AppSidebar />
    <div class="min-w-0 flex-1">
      <AppTopbar />
      <main class="max-w-[1160px] p-7">
        <RouterView />
      </main>
    </div>
  </div>
</template>
