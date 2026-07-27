<script setup lang="ts">
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { Textarea } from '@/components/ui/textarea'
import KeyValueEditor from '@/components/common/KeyValueEditor.vue'
import { authKindOptions } from '@/types/labels'
import type { AuthKind } from '@/types/upstream'
import type { useKeyValueList } from '@/composables/useKeyValueList'

const authKind = defineModel<AuthKind>('authKind', { required: true })

const props = defineProps<{
  headers: ReturnType<typeof useKeyValueList>
  secretInput: string
  hasStoredSecret: boolean
  authConfigInput: string
  hasStoredAuthConfig: boolean
}>()

const emit = defineEmits<{
  'secret-input': [string]
  'secret-clear': []
  'auth-config-input': [string]
  'auth-config-clear': []
}>()
</script>

<template>
  <div class="mb-5 flex flex-col gap-1.5">
    <Label>Method</Label>
    <Select v-model="authKind">
      <SelectTrigger class="w-full">
        <SelectValue />
      </SelectTrigger>
      <SelectContent>
        <SelectItem v-for="option in authKindOptions" :key="option.value" :value="option.value">
          {{ option.label }}
        </SelectItem>
      </SelectContent>
    </Select>
  </div>

  <div v-if="authKind === 'ApiKey'" class="mb-5 flex flex-col gap-1.5">
    <Label>API key</Label>
    <Input
      type="password"
      :model-value="props.secretInput"
      :placeholder="props.hasStoredSecret ? '•••• stored — leave blank to keep' : 'Enter API key'"
      @update:model-value="(v) => emit('secret-input', String(v))"
    />
    <Button
      v-if="props.hasStoredSecret"
      variant="ghost"
      size="sm"
      type="button"
      class="self-start text-destructive"
      @click="emit('secret-clear')"
    >
      Remove stored secret
    </Button>
    <p class="text-xs text-muted-foreground">
      Stored as a secret and redacted when you export the gateway config.
    </p>
  </div>

  <div v-else-if="authKind === 'OAuth2'" class="mb-5 flex flex-col gap-1.5">
    <Label>OAuth2 config (JSON)</Label>
    <Textarea
      :model-value="props.authConfigInput"
      :placeholder="props.hasStoredAuthConfig ? 'stored — leave blank to keep' : '{}'"
      rows="4"
      @update:model-value="(v) => emit('auth-config-input', String(v))"
    />
    <Button
      v-if="props.hasStoredAuthConfig"
      variant="ghost"
      size="sm"
      type="button"
      class="self-start text-destructive"
      @click="emit('auth-config-clear')"
    >
      Remove stored config
    </Button>
  </div>

  <div v-else-if="authKind === 'CustomHeaders'" class="mb-5 flex flex-col gap-1.5">
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
