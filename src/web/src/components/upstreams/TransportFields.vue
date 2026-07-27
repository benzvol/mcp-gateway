<script setup lang="ts">
import KeyValueEditor from '@/components/common/KeyValueEditor.vue'
import StringListEditor from '@/components/common/StringListEditor.vue'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { ToggleGroup, ToggleGroupItem } from '@/components/ui/toggle-group'
import type { TransportKind } from '@/types/upstream'
import type { useKeyValueList } from '@/composables/useKeyValueList'
import type { useStringList } from '@/composables/useStringList'

const transport = defineModel<TransportKind>('transport', { required: true })
const command = defineModel<string>('command', { required: true })
const endpoint = defineModel<string>('endpoint', { required: true })

const props = defineProps<{
  args: ReturnType<typeof useStringList>
  environment: ReturnType<typeof useKeyValueList>
  headers: ReturnType<typeof useKeyValueList>
  commandError?: string
  endpointError?: string
}>()
</script>

<template>
  <div class="mb-5 flex flex-col gap-1.5">
    <Label>Transport</Label>
    <ToggleGroup v-model="transport" type="single" variant="outline" class="justify-start">
      <ToggleGroupItem value="Stdio">stdio</ToggleGroupItem>
      <ToggleGroupItem value="StreamableHttp">Streamable HTTP</ToggleGroupItem>
    </ToggleGroup>
    <p class="text-xs text-muted-foreground">
      stdio launches a local command; HTTP connects to a remote endpoint.
    </p>
  </div>

  <template v-if="transport === 'Stdio'">
    <div class="mb-5 flex flex-col gap-1.5">
      <Label>Command <span class="text-primary">*</span></Label>
      <Input v-model="command" placeholder="npx" :aria-invalid="!!props.commandError" />
      <p v-if="props.commandError" class="text-xs text-destructive">{{ props.commandError }}</p>
    </div>
    <div class="mb-5 flex flex-col gap-1.5">
      <Label>Args</Label>
      <StringListEditor
        :rows="props.args.rows.value"
        @add="props.args.add"
        @remove="props.args.remove"
      />
    </div>
    <div class="mb-5 flex flex-col gap-1.5">
      <Label>Environment variables</Label>
      <KeyValueEditor
        :rows="props.environment.rows.value"
        key-placeholder="Name"
        value-placeholder="Value"
        @add="props.environment.add"
        @remove="props.environment.remove"
      />
    </div>
  </template>

  <template v-else>
    <div class="mb-5 flex flex-col gap-1.5">
      <Label>Endpoint URL <span class="text-primary">*</span></Label>
      <Input
        v-model="endpoint"
        placeholder="https://example.com/mcp"
        :aria-invalid="!!props.endpointError"
      />
      <p v-if="props.endpointError" class="text-xs text-destructive">{{ props.endpointError }}</p>
    </div>
    <div class="mb-5 flex flex-col gap-1.5">
      <Label>Custom headers</Label>
      <KeyValueEditor
        :rows="props.headers.rows.value"
        key-placeholder="Header name"
        value-placeholder="Value"
        @add="props.headers.add"
        @remove="props.headers.remove"
      />
    </div>
  </template>
</template>
